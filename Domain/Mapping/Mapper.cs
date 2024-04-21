using Api.Dto;
using DataAccess;
using DataAccess.Models;
using Domain.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Customer = DataAccess.Models.Customer;
using Transaction = DataAccess.Models.Transaction;
using TransactionStatus = DataAccess.Models.TransactionStatus;

namespace Domain.Mapping;

public static class Mapper
{
    [FromServices] private static TransactionsContext context { get; set; }
    public static CustomerDto MapToDto(Customer customer) => new CustomerDto
    {
        ErrorInfo = null,
        Name = customer.Name,
        Accounts = customer.Accounts != null && customer.Accounts.Count > 0 ? customer.Accounts.Select(MapToDto).ToList() : null,
        Id = customer.Id!
    };

    public static AccountDto MapToDto(Account account) => new AccountDto
    {
        ErrorInfo = null,
        AccountNumber = account.AccountNumber.ToString(),
        OwnerId = account?.Owner?.Id ?? "",
        Amount = account?.Amount ?? 0,
        OutgoingTransactionIds = account?.OutgoingTransactions?.Select(e => e.Id!)?.ToList(),
        IncomingTransactionIds = account?.IncomingTransactions?.Select(e => e.Id!)?.ToList()
    };

    public static TransactionDto MapToDto(Transaction transaction) => new TransactionDto
    {
        Id = transaction.Id,
        ErrorInfo = null,
        SenderAccountNumber = transaction.SenderAccount?.AccountNumber.ToString() ?? "",
        RecipientAccountNumber = transaction.RecipientAccount?.AccountNumber.ToString() ?? "",
        Amount = transaction.Amount,
        Status = transaction.TransactionStatus.Description!
    };

    public static Transaction MapToDb(Aggregates.Transaction transaction)
        => new Transaction
        {
            Id = transaction.TransactionId,
            Amount = transaction.Amount,
        };

    public static DataAccess.Models.TransactionStatus MapToDb(Aggregates.TransactionStatus transactionStatus)
        => new TransactionStatus
        {
            Id = transactionStatus.Id,
            Name = transactionStatus.Name,
            Description = transactionStatus.Description
        };

    public static DataAccess.Models.Account MapToDb(Aggregates.AccountNumber accountNumber, Customer owner)
        => new Account
        {
            Id = accountNumber.Id,
            AccountNumber = long.Parse(accountNumber.Number),
            Owner = owner,
            Amount = accountNumber.GetAmount()
        };


    public static Aggregates.AccountNumber MapToAggregate(Account account) =>
        AccountNumber.CreateNewAccountNumber(account.Id, account.AccountNumber.ToString(), account.Amount);

    public static Aggregates.Customer MapToAggregate(Customer customer) =>
        new Aggregates.Customer(customer.Name, customer.Id, customer.Accounts?.Select(MapToAggregate)?.ToList());
}