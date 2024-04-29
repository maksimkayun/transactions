using Domain.Aggregates.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Utils;

namespace Transactions.Features.Business;

public class TransactionProcessHandler : INotificationHandler<TransactionCreatedDomainEvent>
{
    private TransactionsContext _context;

    public TransactionProcessHandler(TransactionsContext context)
    {
        _context = context;
    }

    public async Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var tr = notification.Transaction;
        var trDb =
            await _context.Transactions
                .Include(t => t.SenderAccount)
                .Include(t => t.RecipientAccount)
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync(x => x.Id == tr.Id.Value.ToString(), cancellationToken);

        if (trDb != default)
        {
            await tr.MakeTransaction(cancellationToken);

            trDb.SenderAccount.Amount = tr.SenderAccount.Amount;
            trDb.RecipientAccount.Amount = tr.RecipientAccount.Amount;
            trDb.TransactionStatus = await
                _context.TransactionStatuses.AsNoTrackingWithIdentityResolution()
                    .FirstAsync(e => e.Id == tr.Status.Id, cancellationToken);

            _context.Transactions.Update(trDb);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}