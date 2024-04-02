
using System.Net;
using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Transactions.Aggregates;
using Transactions.DomainServices;
using Transactions.DomainServices.Implementations;
using Transactions.Dto;
using Transactions.Dto.CreateDto;
using Transactions.Infrastructure;
using Transactions.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionstring = "Host=localhost;Port=5432;Database=transactions;Username=postgres;Password=postgres;";
builder.Services.AddDbContext<TransactionsContext>(opt =>
{
    opt.UseNpgsql(connectionstring);
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    q.AddJob<TransactionProcessor>(TransactionProcessor.Key);

    q.AddTrigger(opts => opts
        .ForJob(TransactionProcessor.Key)
        .WithIdentity("TransactionProcessor-startTrigger")
        .WithSimpleSchedule(x => x         
            .WithIntervalInMinutes(1)
            .RepeatForever())
        .StartNow()
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandleMiddleware>();

app.MapGet("/customer", async Task<Results<Ok<List<CustomerDto>>, BadRequest<List<CustomerDto>>>> (
        [FromQuery] int take,
        [FromQuery] int skip,
        CancellationToken cancellationToken, [FromServices] ICustomerService customerService) =>
    {
        var result = await customerService.GetCustomers(take, skip, cancellationToken);
        return result == null || result.Any(e=>e.HasError) ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
    })
    .WithName("GetCustomers")
    .Produces<List<CustomerDto>>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapGet("/customer/{id}", async Task<Results<Ok<CustomerDto>, BadRequest<CustomerDto>>> (string id,
        CancellationToken cancellationToken, [FromServices] ICustomerService customerService) =>
    {
        var result = await customerService.FindCustomer(id, cancellationToken);
        return result == null || result.HasError ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
    })
    .WithName("GetCustomerById")
    .Produces<CustomerDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/customer/create", async Task<Results<Ok<CustomerDto>, BadRequest<CustomerDto>>> ([FromBody] CreateCustomerDto customer,
        CancellationToken cancellationToken, [FromServices] ICustomerService customerService) =>
    {
        var result = await customerService.RegisterCustomer(customer, cancellationToken);
        return result == null || result.HasError ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
    })
    .WithName("RegisterCustomer")
    .Produces<CustomerDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/customer/{customerId}/openaccount",
        async Task<Results<Ok<AccountDto>, BadRequest<AccountDto>>> (string customerId,
            CancellationToken cancellationToken, [FromServices] ICustomerService customerService) =>
        {
            var result = await customerService.OpenAccount(customerId, cancellationToken);
            return result == null || result.HasError ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
        })
    .WithName("OpenAccount")
    .Produces<AccountDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/transactions/sendmoney/",
        async Task<Results<Ok<TransactionDto>, BadRequest<TransactionDto>>> (
            
            [FromQuery] string senderAccountNumber,
            [FromQuery] string recipientAccountNumber,
            [FromQuery] decimal amount,
            CancellationToken cancellationToken, [FromServices] ITransactionService transactionService) =>
        {
            var result = await transactionService.MakeTransaction(senderAccountNumber, recipientAccountNumber, amount, cancellationToken);
            return result == null || result.HasError ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
        })
    .WithName("SendMoney")
    .Produces<TransactionDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapGet("/transactions/{transactionId}",
        async Task<Results<Ok<TransactionDto>, BadRequest<TransactionDto>>> (
            string transactionId,
            CancellationToken cancellationToken, [FromServices] ITransactionService transactionService) =>
        {
            var result = await transactionService.GetTransactionById(transactionId, cancellationToken);
            return result == null || result.HasError ? TypedResults.BadRequest(result) : TypedResults.Ok(result);
        })
    .WithName("GetTransactionById")
    .Produces<TransactionDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();




app.Run();