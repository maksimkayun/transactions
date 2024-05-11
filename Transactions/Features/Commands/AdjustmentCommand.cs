using Api.Dto;
using Domain.Aggregates;
using Domain.Aggregates.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Commands;

public record AdjustmentCommand(decimal Amount, string AccountNumber, string Mode): IRequest<AdjustmentResult>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record AdjustmentResult(Account Account, ErrorInfo? ErrorInfo);

public class AdjustmentCommandHandler : IRequestHandler<AdjustmentCommand, AdjustmentResult>
{
    private TransactionsContext _context;
    private EventsExecutor _executor;

    public AdjustmentCommandHandler(TransactionsContext context, EventsExecutor executor)
    {
        _context = context;
        _executor = executor;
    }

    public async Task<AdjustmentResult> Handle(AdjustmentCommand request, CancellationToken cancellationToken)
    {
        if (!long.TryParse(request.AccountNumber, out var accNum))
        {
            throw new InvalidAccountNumberException();
        }

        var acc = await _context.Accounts.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(e => e.AccountNumber == accNum && !e.IsDeleted, cancellationToken);

        if (acc != default)
        {
            var account = Account.Create(AccountId.Of(acc.Id), AccountNumber.Of(acc.AccountNumber), acc.Amount);
            if (request.Mode == "+")
                account.Increase(request.Amount);
            else if (request.Mode == "-")
                account.Decrease(request.Amount);

            acc.Amount = account.Amount;
            _context.Accounts.Update(acc);
            await _context.SaveChangesAsync(cancellationToken);

            await _executor.ExecuteEvents(account, cancellationToken);

            return new AdjustmentResult(account, null);
        }

        throw new AccountDoesNotExistException(accNum);
    }
}