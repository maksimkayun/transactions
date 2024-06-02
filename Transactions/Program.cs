using System.Reflection;
using Api.Dto;
using Api.Dto.CreateDto;
using Domain.Aggregates;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Transactions.DataAccess;
using Transactions.EsbAdapter.EventBus;
using Transactions.Features.Commands;
using Transactions.Features.Queries;
using Transactions.Infrastructure;
using Transactions.Jobs;
using Transactions.StartupExtensions;
using Transactions.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var appSettings = builder.UseAppSettings();

builder.Services.AddDbContext<TransactionsContext>(opt =>
{
    opt.UseNpgsql(appSettings.ConnectionString);
}, ServiceLifetime.Transient);

builder.Services.AddMediatR(e => e.RegisterServicesFromAssemblies(Assembly.GetEntryAssembly()));
builder.Services.AddTransient<EventsExecutor>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Just use the name of your job that you created in the Jobs folder.
    q.AddJob<TransactionProcessor>(TransactionProcessor.Key);

    q.AddTrigger(opts => opts
        .ForJob(TransactionProcessor.Key)
        .WithIdentity("TransactionProcessor-startTrigger")
        .StartNow()
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddSingleton<KafkaProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandleMiddleware>();

app.MapGet("/customer/{id}", async Task<Results<Ok<CustomerDto>, BadRequest<CustomerDto>, NotFound>> ([FromRoute] string id,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var query = new GetCustomerQuery(id, null);
        var result = await mediator.Send(query,cancellationToken);
        return result.Any(e=>e.HasError) ? TypedResults.BadRequest(result.First()) : result.Count > 0 ? TypedResults.Ok(result.First()) : TypedResults.NotFound();
    })
    .WithName("GetCustomerById")
    .Produces(200)
    .Produces(400)
    .Produces(404)
    .Produces(500)
    .WithOpenApi();

app.MapGet("/customer/name/{name}", async Task<Results<Ok<List<CustomerDto>>, BadRequest<CustomerDto>>> ([FromRoute] string name,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var query = new GetCustomerQuery(null, name);
        var result = await mediator.Send(query,cancellationToken);
        return result.Any(e=>e.HasError) ? TypedResults.BadRequest(result.First()) : TypedResults.Ok(result);
    })
    .WithName("GetCustomerByName")
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapGet("/customer/list", async Task<Results<Ok<List<CustomerDto>>, BadRequest<CustomerDto>>> ([FromQuery] int? skip, [FromQuery] int? take,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var query = new GetCustomerQuery(null, null, skip, take);
        var result = await mediator.Send(query,cancellationToken);
        return result.Any(e=>e.HasError) ? TypedResults.BadRequest(result.First()) : TypedResults.Ok(result);
    })
    .WithName("GetCustomerList")
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/customer/create", async ([FromBody] CreateCustomerDto customer,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var command = new CreateCustomerCommand(customer.Name);
        var response = await mediator.Send(command, cancellationToken);
        return Results.Created($"/customer/{response.Customer.Id.Value}", response);
    })
    .WithName("RegisterCustomer")
    .Produces(201)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/customer/{customerId}/close", async Task<Results<Ok<Customer>, BadRequest<Customer>>>  (string customerId,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var req = new CloseCustomerProfileCommand(customerId);
        var result = await mediator.Send(req, cancellationToken);

        return TypedResults.Ok(result.Customer);
    })
    .WithName("CloseCustomerProfile")
    .Produces(201)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/customer/{customerId}/openaccount/{startAmount}",
        async Task<Results<Ok<Account>, BadRequest<AccountDto>>> (string customerId, decimal startAmount,
            CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
        {
            var findCustQuery = new GetCustomerQuery(customerId, null);
            var customer = await mediator.Send(findCustQuery, cancellationToken);
            if (customer.Any(e=>e.HasError))
            {
                var accError = ErrorDtoCreator.Create<AccountDto>(customer.First().ErrorInfo!.Message);
                return TypedResults.BadRequest(accError);
            }

            var openAccountCommand = new OpenAccountCommand(customer.First(), startAmount);
            var openedAcc = await mediator.Send(openAccountCommand, cancellationToken);

            return TypedResults.Ok(openedAcc.Account);
        })
    .WithName("OpenAccount")
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/account/closeaccount/{accountNumber}",
        async Task<Results<Ok<Account>, BadRequest<ErrorInfo>>> (long accountNumber, [FromQuery] long refundAccountNumber,
            CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
        {
            var req = new CloseAccountCommand(accountNumber, refundAccountNumber);
            var result = await mediator.Send(req, cancellationToken);

            return result.ErrorInfo != null
                ? TypedResults.BadRequest(result.ErrorInfo)
                : TypedResults.Ok(result.Account);
        })
    .WithName("ClosenAccount")
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/account/{accountNumber}/adjustment/{amount}/{mode}",
        async Task<Results<Ok<Account>, BadRequest>> (string accountNumber, decimal amount, string mode,
            CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
        {
            var command = new AdjustmentCommand(amount, accountNumber, mode);
            var result = await mediator.Send(command, cancellationToken);
            return TypedResults.Ok(result.Account);
        })
    .WithName("Adjustment")
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapPost("/transactions/sendmoney/",
        async Task<Results<Ok<SendMoneyCommandResult>, BadRequest<TransactionDto>>> (
            
            [FromQuery] string senderAccountNumber,
            [FromQuery] string recipientAccountNumber,
            [FromQuery] decimal amount,
            CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
        {
            var tr = await mediator.Send(new SendMoneyCommand(senderAccountNumber, recipientAccountNumber, amount));
            return TypedResults.Ok(tr);
        })
    .WithName("SendMoney")
    .Produces<TransactionDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.MapGet("/transactions/getstatus/{id}",async Task<Results<Ok<TransactionDto>, BadRequest<TransactionDto>>> (
        [FromRoute] string id,
        CancellationToken cancellationToken, [FromServices] IMediator mediator) =>
    {
        var tr = await mediator.Send(new GetTransactionStatusQuery(id), cancellationToken);
        return tr.HasError ? TypedResults.BadRequest(tr) : TypedResults.Ok(tr);
    })
    .WithName("GetTransactionStatus")
    .Produces<TransactionDto>()
    .Produces(200)
    .Produces(400)
    .Produces(500)
    .WithOpenApi();

app.Run();