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
    private readonly IMediator _mediator;

    public CloseCustomerProfileCommandHandler(TransactionsContext context, EventsExecutor executor, IMediator mediator)
    {
        _context = context;
        _executor = executor;
        _mediator = mediator;
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

        foreach (var account in cust.Accounts)
        {
            if (!account.IsDeleted)
            {
                var closeAccountCommand = new CloseAccountCommand(account.AccountNumber, 0);
                var closeAccountResult = await _mediator.Send(closeAccountCommand, cancellationToken);

                if (closeAccountResult.ErrorInfo != null)
                {
                    throw new Exception($"Error closing account: {closeAccountResult.ErrorInfo.Message}");
                }
            }
        }

        cust.IsDeleted = true;
        _context.Customers.Update(cust);

        await _context.SaveChangesAsync(cancellationToken);

        var customer = MappingService.CustomerFromDb(cust);
        await _executor.ExecuteEvents(customer, cancellationToken);

        return new CloseCustomerProfileResult(customer);
    }
}