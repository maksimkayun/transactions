using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record CloseAccountCommand(string CustomerId, long AccountNumber) : IRequest<CloseAccountResult>;

public record CloseAccountResult(AccountDto AccountDto);

public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, CloseAccountResult>
{
    private readonly TransactionsContext _context;
    private EventsExecutor _executor;

    public CloseAccountCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<CloseAccountResult> Handle(CloseAccountCommand request, CancellationToken cancellationToken)
    {
        var cust = await _context.Customers
            .AsNoTrackingWithIdentityResolution()
            .Include(c => c.Accounts.Where(a => !a.IsDeleted)).ThenInclude(account => account.OutgoingTransactions)
            .Include(c => c.Accounts.Where(a => !a.IsDeleted)).ThenInclude(account => account.IncomingTransactions)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, cancellationToken);
        if (cust == default)
        {
            throw new CustomerNotFoundException();
        }

        var acc = _context.Accounts.Any()
            ? await _context.Accounts
                ?.Where(e => !e.IsDeleted)
                .Include(e => e.IncomingTransactions)
                .Include(e => e.OutgoingTransactions)
                ?.AsNoTrackingWithIdentityResolution()
                ?.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber, cancellationToken)!
            : null;
        if (acc == default)
        {
            throw new AccountDoesNotExistException(request.AccountNumber);
        }

        var customer = MappingService.CustomerFromDb(cust);
        customer.CloseAccount(acc.AccountNumber);

        cust = MappingService.CustomerMapAggregateToDb(customer);
        await _context.Accounts?.Where(e => e.AccountNumber == request.AccountNumber)
            ?.ExecuteUpdateAsync(e => e.SetProperty(x => x.IsDeleted, true), cancellationToken)!;
        await _context.SaveChangesAsync(cancellationToken);
        await _executor.ExecuteEvents(customer, cancellationToken);

        return new CloseAccountResult(new AccountDto
        {
            ErrorInfo = null,
            AccountNumber = acc.AccountNumber.ToString(),
            OwnerId = cust.Id,
            Amount = acc.Amount,
            OutgoingTransactionIds = acc.OutgoingTransactions?.Select(e => e.Id)?.ToList() ?? new List<string>(),
            IncomingTransactionIds = acc.IncomingTransactions?.Select(e => e.Id)?.ToList() ?? new List<string>()
        });
    }
}