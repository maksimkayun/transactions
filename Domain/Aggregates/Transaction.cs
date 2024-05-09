using Domain.Aggregates.Common;
using Domain.Aggregates.Events;
using Domain.Aggregates.Exceptions;
using MassTransit;
using IAggregate = Domain.Common.IAggregate;

namespace Domain.Aggregates;

public class Transaction : Aggregate<TransactionId>, IAggregate
{
    public TransactionStatus Status { get; private set; }
    
    public Account RecipientAccount { get; private set; }
    public Account SenderAccount { get; private set; }
    
    public decimal Amount { get; private set; }
    private TransactionResult Result;
    
    public DateTime CreatedDate { get; private set; }

    public static Transaction Create(Account senderAccount, Account recipientAccount, decimal amount, DateTime? dateTime = null)
    {
        if (senderAccount.Number.Value == recipientAccount.Number.Value)
        {
            throw new AccountsMatchException("Лицевые счета совпадают, транзакция невозможна");
        }
        
        var tr = new Transaction
        {
            Id = TransactionId.Of(NewId.NextGuid()),
            IsDeleted = false,
            Status = TransactionStatus.Created,
            RecipientAccount = recipientAccount,
            SenderAccount = senderAccount,
            Amount = amount > 0 ? amount : throw new IncorrectTransferAmountException("Сумма для перевода должна быть положительным числом!"),
            CreatedDate = dateTime ?? DateTime.UtcNow
        };

        var @event = new TransactionCreatedDomainEvent(tr);
        tr.AddDomainEvent(@event);

        return tr;
    }

    public async Task<TransactionStatus> MakeTransaction(CancellationToken cancellationToken)
    {
        if (Equals(Status, TransactionStatus.Created))
        {
            try
            {
                var senderRemains = SenderAccount.Decrease(Amount);
                var recipientRemains = RecipientAccount.Increase(Amount);
                Result = new TransactionResult(senderRemains, recipientRemains, Status);
                
                Result.UpdateStatus(RollingStatus());
            }
            catch (Exception e)
            {
                var senderRemains = SenderAccount.Amount;
                var recipientRemains = RecipientAccount.Amount;
                Result = new TransactionResult(senderRemains, recipientRemains, Status);
                Result.UpdateStatus(CancelTransaction(null));
            }
        }

        if (Equals(Status, TransactionStatus.Processing))
        {
            Result.UpdateStatus(RollingStatus());
        }

        return Status;
    }
    
    

    public TransactionStatus RollingStatus()
    {
        if (Equals(Status, TransactionStatus.Created))
        {
            Status = TransactionStatus.Processing;
        }

        if (Equals(Status, TransactionStatus.Processing))
        {
            Status = TransactionStatus.Completed;
        }

        if (Equals(Status, TransactionStatus.Cancelled))
        {
            Status = TransactionStatus.Created;
        }

        return Status;
    }

    public TransactionStatus CancelTransaction(string? reason)
    {
        Status = TransactionStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Status.WithReason(reason);
        }

        return Status;
    }

    public TransactionResult GetResult() => Result.WithCheck();
}