using Common.Settings;
using Domain.Aggregates.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Transactions.DataAccess;
using Transactions.EsbAdapter.EventBus;

namespace Transactions.Features.Business;

public class TransactionProcessHandler : INotificationHandler<TransactionCreatedDomainEvent>
{
    private TransactionsContext _context;
    private KafkaProducerService _kafkaProducerService;
    private AppSettings _options;

    public TransactionProcessHandler(TransactionsContext context, KafkaProducerService kafkaProducerService, AppSettings options)
    {
        _context = context;
        _kafkaProducerService = kafkaProducerService;
        _options = options;
    }
    
    public record TransactionCreatedMessage(Guid Id, string SenderAccount, string RecipientAccount, string Amount, string timestamp);

    public async Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var tr = notification.Transaction;

        if (_options.UseSelfTransactionProcessor)
        {
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
        else
        {

            var message = new TransactionCreatedMessage(tr.Id.Value, tr.SenderAccount.Number.Value.ToString(),
                tr.RecipientAccount.Number.Value.ToString(), tr.Amount.ToString(),
                new DateTimeOffset(tr.CreatedDate.ToUniversalTime()).ToUnixTimeSeconds().ToString()
            );
            await _kafkaProducerService.ProduceAsync(JsonConvert.SerializeObject(message), cancellationToken);
        }
    }
}