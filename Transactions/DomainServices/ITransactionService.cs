using Transactions.Dto;

namespace Transactions.DomainServices;

public interface ITransactionService
{
    public Task<TransactionDto?> MakeTransaction(string senderAccountNumber, string recipientAccountNumber,
        decimal amount, CancellationToken cancellationToken);

    public Task<TransactionDto?> GetTransactionById(string transactionId, CancellationToken cancellationToken);
}