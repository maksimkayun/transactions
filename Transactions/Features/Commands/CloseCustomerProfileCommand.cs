using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record CloseCustomerProfileCommand(string CustomerId) : IRequest<CloseCustomerProfileResult>;

public record CloseCustomerProfileResult(Customer Customer);

public class CloseCustomerProfileCommandHandler : IRequestHandler<CloseCustomerProfileCommand, CloseCustomerProfileResult>
{
    private readonly TransactionsContext _context;
    private readonly EventsExecutor _executor;

    public CloseCustomerProfileCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<CloseCustomerProfileResult> Handle(CloseCustomerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var cust = await _context.Customers.Include(c => c.Accounts).ThenInclude(a => a.IncomingTransactions)
            .Include(c => c.Accounts).ThenInclude(a => a.OutgoingTransactions)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, cancellationToken);

        if (cust == default)
        {
            throw new CustomerNotFoundException();
        }

        cust.IsDeleted = true;
        _context.Customers.Update(cust);

        await _context.SaveChangesAsync(cancellationToken);

        var customer = MappingService.CustomerFromDb(cust);

        return new CloseCustomerProfileResult(customer);
    }
}