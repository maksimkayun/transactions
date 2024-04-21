using Domain.Aggregates.Common;

namespace Domain.Aggregates;

public class Transaction : IAggregate<Transaction>
{
    public bool IsDeleted { get; private set; }
    public Transaction Id { get; private set; }
    public string TransactionId { get; }
    public TransactionStatus Status { get; private set; }
    
    public AccountNumber RecipientAccountNumber { get; private set; }
    public AccountNumber SenderAccountNumber { get; private set; }
    
    public decimal Amount { get; private set; }
    private TransactionResult Result;
    
    public Transaction(AccountNumber recipientAccountNumber, AccountNumber senderAccountNumber, decimal amount, string? transactionId = null)
    {
        Id = this;
        TransactionId = transactionId ?? Guid.NewGuid().ToString();
        Status = TransactionStatus.Created;

        if (recipientAccountNumber.Equals(senderAccountNumber))
        {
            throw new Exception("Лицевые счета совпадают, транзакция невозможна");
        }
        RecipientAccountNumber = recipientAccountNumber;
        SenderAccountNumber = senderAccountNumber;

        Amount = amount > 0 ? amount : throw new Exception("Сумма для перевода должна быть положительным числом!");
    }

    public async Task<TransactionStatus> MakeTransaction(CancellationToken cancellationToken)
    {
        // if (cancellationToken.CanBeCanceled)
        // {
        //     CancelTransaction(null);
        //     Result = new TransactionResult(SenderAccountNumber.Decrease(Amount),
        //         RecipientAccountNumber.Increase(Amount), Status);
        //     return Status;
        // }
        
        if (Equals(Status, TransactionStatus.Created))
        {
            try
            {
                var senderRemains = SenderAccountNumber.Decrease(Amount);
                var recipientRemains = RecipientAccountNumber.Increase(Amount);
                Result = new TransactionResult(senderRemains, recipientRemains, Status);
                
                Result.UpdateStatus(RollingStatus());
                Thread.Sleep(new Random().Next(1000, 2000));
            }
            catch (Exception e)
            {
                var senderRemains = SenderAccountNumber.GetAmount();
                var recipientRemains = RecipientAccountNumber.GetAmount();
                Result = new TransactionResult(senderRemains, recipientRemains, Status);
                Result.UpdateStatus(CancelTransaction(null));
            }
        }

        if (Equals(Status, TransactionStatus.Processing))
        {
            Result.UpdateStatus(RollingStatus());
            Thread.Sleep(new Random().Next(1000, 2000));
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