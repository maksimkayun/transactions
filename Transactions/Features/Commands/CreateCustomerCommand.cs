using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MassTransit;
using MediatR;

namespace Transactions.Features.Commands;

public record CreateCustomerCommand(string Name) : IRequest<CreateCustomerResult>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateCustomerResult(Customer Customer);

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly TransactionsContext _context;

    public CreateCustomerCommandHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var cust = _context.Customers.FirstOrDefault(e => e.Name == request.Name);
        if (cust is not null)
        {
            throw new CustomerAlreadyExistException();
        }

        var custEntity = _context.Customers.Add(Customer.Create(CustomerId.Of(request.Id), request.Name)).Entity;
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResult(custEntity);
    }
}