using Api.Dto;

namespace Domain.Services;

public interface ITransactionService
{
    public Task<TransactionDto?> MakeTransaction(string senderAccountNumber, string recipientAccountNumber,
        decimal amount, CancellationToken cancellationToken);

    public Task<TransactionDto?> GetTransactionById(string transactionId, CancellationToken cancellationToken);

    /// <summary>
    /// Начисление
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<AccountDto?> Deposit(string accountNumber, decimal amount, CancellationToken cancellationToken);
    
    /// <summary>
    /// Списание
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<AccountDto?> Debit(string? accountNumber, decimal amount, CancellationToken cancellationToken);
}