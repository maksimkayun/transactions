using Api.Dto;
using Consul;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Transactions.Infrastructure;
using Transactions.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.UseAppSettings();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://localhost:8500");
}));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseConsul();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/aggregation/customer/{name}", async (string name) =>
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7169/api/tr/customer/name/{name}");
        var custResponse = await client.SendAsync(request);

        var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(await custResponse.Content.ReadAsStringAsync());
        var trIds = customers?
            .SelectMany(x =>
            {
                return x.Accounts?.SelectMany(t =>
                       {
                           if (t.OutgoingTransactionIds != null)
                               return t.IncomingTransactionIds?.Concat(t.OutgoingTransactionIds) ??
                                      Array.Empty<string>();
                           return Array.Empty<string>();
                       }) ??
                       Array.Empty<string>();
            }).Distinct();

        List<TransactionDto> dtos = new List<TransactionDto>(trIds?.Count() ?? 0);
        if (trIds != null)
            foreach (var tr in trIds)
            {
                request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://localhost:7169/api/tr/transactions/getstatus/{tr}");
                var response = await client.SendAsync(request);
                var trResp = JsonConvert.DeserializeObject<TransactionDto>(await response.Content.ReadAsStringAsync());
                dtos.Add(trResp);
            }

        return TypedResults.Ok(new
        {
            Customers = customers,
            Transactions = dtos
        });
    })
    .WithName("AggreagateCustomerInfo")
    .WithOpenApi();

app.Run();