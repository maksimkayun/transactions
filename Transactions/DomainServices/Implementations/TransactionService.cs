using DataAccess;
using Microsoft.EntityFrameworkCore;
using Transactions.Dto;
using Transactions.Mapping;

namespace Transactions.DomainServices.Implementations;

public class TransactionService(TransactionsContext context) : ITransactionService
{
    private readonly TransactionsContext _context = context;

    public async Task<TransactionDto?> MakeTransaction(string senderAccountNumber, string recipientAccountNumber, decimal amount, CancellationToken cancellationToken)
    {
        var senderAcc = await
            context.Accounts.FirstOrDefaultAsync(e => e.AccountNumber == long.Parse(senderAccountNumber), cancellationToken);

        var recipientAcc =
            await context.Accounts.FirstOrDefaultAsync(e => e.AccountNumber == long.Parse(recipientAccountNumber),
                cancellationToken);

        if (senderAcc == null && recipientAcc == null)
        {
            return ErrorDtoCreator.Create<TransactionDto?>("Лицевые счета отправителя и получателя не найдены");
        }

        if (senderAcc == null)
        {
            return ErrorDtoCreator.Create<TransactionDto?>("Лицевой счет отправителя не найден");
        }
        
        if (recipientAcc == null)
        {
            return ErrorDtoCreator.Create<TransactionDto?>("Лицевой счет получателя не найден");
        }

        var transaction =
            new Aggregates.Transaction(Mapper.MapToAggregate(recipientAcc), Mapper.MapToAggregate(senderAcc), amount);

        var trToSave = Mapper.MapToDb(transaction);
        trToSave.RecipientAccount = recipientAcc;
        trToSave.SenderAccount = senderAcc;
        trToSave.TransactionStatus = await context.TransactionStatuses.FirstAsync(e => e.Id == transaction.Status.Id, cancellationToken);
        await context.Transactions.AddAsync(trToSave, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var trDb = await context.Transactions
            .Include(e=>e.SenderAccount)
            .Include(e=>e.RecipientAccount)
            .FirstOrDefaultAsync(e => e.Id == transaction.TransactionId, cancellationToken);
        if (trDb == null)
        {
            return ErrorDtoCreator.Create<TransactionDto?>("Транзакция не создана");
        }
        var result = Mapper.MapToDto(trDb);

        return result;
    }

    public async Task<TransactionDto?> GetTransactionById(string transactionId, CancellationToken cancellationToken)
    {
        var result = await context.Transactions
            .Include(e => e.TransactionStatus)
            .Include(e => e.SenderAccount)
            .Include(e => e.RecipientAccount)
            .FirstOrDefaultAsync(e => e.Id == transactionId, cancellationToken);

        if (result == null)
        {
            return ErrorDtoCreator.Create<TransactionDto?>("Транзакция не найдена");
        }

        var dto = Mapper.MapToDto(result);
        return dto;
    }
}