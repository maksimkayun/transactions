using System.Linq.Expressions;
using Api.Dto;
using DataAccess;
using Domain.Aggregates;
using Domain.Events;
using Domain.Mapping;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services.Implementations;

public class TransactionService(TransactionsContext context, IMediator mediator) : ITransactionService
{
    private readonly TransactionsContext _context = context;
    private IMediator _mediator = mediator;

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
        
        var result = Mapper.MapToDto(trToSave);

        await _mediator.Publish(new TransactionCreated(transaction), cancellationToken);

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

    /// <inheritdoc cref="ITransactionService"/>
    public async Task<AccountDto?> Deposit(string accountNumber, decimal amount, CancellationToken cancellationToken)
    {
        var result = await DebitDeposit(accountNumber, amount, acc=>acc.Increase(amount), cancellationToken);
        return result;
    }

    /// <inheritdoc cref="ITransactionService"/>
    public async Task<AccountDto?> Debit(string? accountNumber, decimal amount, CancellationToken cancellationToken)
    {
        var result = await DebitDeposit(accountNumber, amount, acc=>acc.Decrease(amount), cancellationToken);
        return result;
    }

    private async Task<AccountDto?> DebitDeposit(string? accountNumber, decimal amount, Expression<Func<AccountNumber, decimal>> expression,
        CancellationToken cancellationToken)
    {
        var dbCustomer =
            _context.Customers.Include(c=>c.Accounts).FirstOrDefault(e =>
                e.Accounts.Any(acc => acc.AccountNumber.ToString() == accountNumber));
        var dbAccountNumber = _context.Accounts.FirstOrDefault(e => e.AccountNumber.ToString() == accountNumber);
        if (dbAccountNumber == null)
        {
            return ErrorDtoCreator.Create<AccountDto?>($"ЛС с номером {accountNumber} не найден");
        }
        if (dbCustomer == null)
        {
            return ErrorDtoCreator.Create<AccountDto?>($"Владелец ЛС с номером {accountNumber} не найден");
        }
        
        var customer =
            Mapper.MapToAggregate(dbCustomer);
        var accountNumberValObj = AccountNumber.CreateAccountNumber(accountNumber, customer);

        expression.Compile().Invoke(accountNumberValObj);

        dbAccountNumber.Amount = accountNumberValObj.GetAmount();
        dbAccountNumber = _context.Accounts.Update(dbAccountNumber).Entity;
        await _context.SaveChangesAsync(cancellationToken);

        return Mapper.MapToDto(dbAccountNumber);
    }
}