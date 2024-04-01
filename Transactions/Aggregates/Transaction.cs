using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class Transaction : IAggregate<Transaction>
{
    public bool IsDeleted { get; private set; }
    public Transaction Id { get; private set; }
    public TransactionStatus Status { get; private set; }
    
    public AccountNumber RecipientAccountNumber { get; private set; }
    public AccountNumber SenderAccountNumber { get; private set; }
    
    public decimal Amount { get; private set; }
    private TransactionResult Result;
    
    public Transaction(AccountNumber recipientAccountNumber, AccountNumber senderAccountNumber, decimal amount)
    {
        Id = this;
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
        if (cancellationToken.CanBeCanceled)
        {
            CancelTransaction(null);
            Result = new TransactionResult(SenderAccountNumber.Decrease(Amount),
                RecipientAccountNumber.Increase(Amount), Status);
            return Status;
        }
        
        if (Equals(Status, TransactionStatus.Created))
        {
            try
            {
                Result = new TransactionResult(SenderAccountNumber.Decrease(Amount),
                    RecipientAccountNumber.Increase(Amount), Status);
                
                Result.UpdateStatus(RollingStatus());
                Thread.Sleep(new Random().Next(1000, 2000));
            }
            catch (Exception e)
            {
                Result.UpdateStatus(CancelTransaction(e.Message));
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
