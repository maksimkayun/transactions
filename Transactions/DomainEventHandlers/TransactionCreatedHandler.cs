using DataAccess;
using Domain.Aggregates;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Transactions.DomainEventHandlers;

public class TransactionCreatedHandler : INotificationHandler<TransactionCreated>
{
    private IMediator _mediator;
    private TransactionsContext _context;

    public TransactionCreatedHandler(IMediator mediator, TransactionsContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task Handle(TransactionCreated notification, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = notification.Transaction;
            
            await transaction.MakeTransaction(CancellationToken.None);
            var result = transaction.GetResult().WithCheck();

            var dbTransaction = await _context.Transactions.SingleAsync(e => e.Id == transaction.TransactionId, cancellationToken);
            var dbSenderAcc = await
                _context.Accounts.SingleAsync(e => e.AccountNumber.ToString() == transaction.SenderAccountNumber.Number,
                    cancellationToken);
            var dbRecipientAcc = await
                _context.Accounts.SingleAsync(e => e.AccountNumber.ToString() == transaction.RecipientAccountNumber.Number,
                    cancellationToken);

            dbSenderAcc.Amount = transaction.SenderAccountNumber.GetAmount();
            dbRecipientAcc.Amount = transaction.RecipientAccountNumber.GetAmount();
        
            dbTransaction.TransactionStatus =
                await _context.TransactionStatuses.SingleAsync(e => e.Id == transaction.Status.Id, cancellationToken);
            _context.Transactions.Update(dbTransaction);
            _context.Accounts.UpdateRange([dbSenderAcc, dbRecipientAcc]);
            await _context.SaveChangesAsync(cancellationToken);
        
            if (result.TransactionStatus == TransactionStatus.Completed)
            {
                await _mediator.Publish(new TransactionProcessed(result),cancellationToken);
            }
            else
            {
                await _mediator.Publish(new TransactionCancelled(result),cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }
}