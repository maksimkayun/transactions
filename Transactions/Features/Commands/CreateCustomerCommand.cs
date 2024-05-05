using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record CreateCustomerCommand(string Name) : IRequest<CreateCustomerResult>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateCustomerResult(Customer Customer, ErrorInfo? ErrorInfo);

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly TransactionsContext _context;
    private readonly EventsExecutor _executor;

    public CreateCustomerCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var cust = await _context.Customers
            .Include(customer => customer.Accounts)
            .ThenInclude(account => account.OutgoingTransactions)
            .Include(customer => customer.Accounts)
            .ThenInclude(account => account.IncomingTransactions)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);
        if (cust is not null && !cust.IsDeleted)
        {
            throw new CustomerAlreadyExistException();
        }

        Customer customer;
        if (cust.IsDeleted)
        {
            cust.IsDeleted = false;
            _context.Customers.Update(cust);
            await _context.SaveChangesAsync(cancellationToken);
            customer = MappingService.CustomerFromDb(cust);
        }
        else
        {
            customer = Customer.Create(CustomerId.Of(NewId.NextGuid()), request.Name);
            cust = MappingService.CustomerMapAggregateToDb(customer);
            await _context.Customers.AddAsync(cust, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _executor.ExecuteEvents(customer, cancellationToken);
        }


        return new CreateCustomerResult(customer, null);
    }
}