using Domain.Aggregates;
using Account = Transactions.DataAccess.Models.Account;
using Customer = Transactions.DataAccess.Models.Customer;
using Transaction = Transactions.DataAccess.Models.Transaction;
using TransactionStatus = Transactions.DataAccess.Models.TransactionStatus;

namespace Transactions.Utils;

public static class MappingService
{
    public static Customer CustomerMapAggregateToDb(Domain.Aggregates.Customer customer)
        => new Customer
        {
            Id = customer.Id.Value.ToString(),
            Name = customer.Name,
            Accounts = customer.Accounts.Select(acc => new Account
            {
                Id = acc.Id.Value.ToString(),
                AccountNumber = acc.Number.Value,
                OutgoingTransactions = acc.OutgoingTransactions.Select(x => TransactionMapAggregateToDb(x)).ToList(),
                IncomingTransactions = acc.IncomingTransactions.Select(x => TransactionMapAggregateToDb(x)).ToList(),
                Amount = acc.Amount
            }).ToList()
        };

    public static Transaction TransactionMapAggregateToDb(Domain.Aggregates.Transaction tr)
        => new Transaction
        {
            Id = tr.Id.Value.ToString(),
            Amount = tr.Amount,
            SenderAccount = new Account()
            {
                Id = tr.SenderAccount.Id.Value.ToString()
            },
            RecipientAccount = new Account()
            {
                Id = tr.RecipientAccount.Id.Value.ToString()
            },
            TransactionStatus = new TransactionStatus()
            {
                Id = tr.Status.Id
            },
            CreatedDate = tr.CreatedDate
        };

    public static Account AccountMapAggregateToDb(Domain.Aggregates.Account acc, Customer owner)
    {
        var accountDb = new Account
        {
            Id = acc.Id.Value.ToString(),
            AccountNumber = acc.Number.Value,
            Owner = owner,
            Amount = acc.Amount,
            OpenDate = acc.OpenDate,
            IsDeleted = acc.IsDeleted
        };

        if (acc.OutgoingTransactions != null && acc.OutgoingTransactions.Any())
        {
            accountDb.OutgoingTransactions =
                acc.OutgoingTransactions.Select(x => TransactionMapAggregateToDb(x)).ToList();
        }

        if (acc.IncomingTransactions != null && acc.IncomingTransactions.Any())
        {
            accountDb.IncomingTransactions =
                acc.IncomingTransactions.Select(x => TransactionMapAggregateToDb(x)).ToList();
        }

        return accountDb;
    }

    public static Domain.Aggregates.Account AccountFromDb(Account account)
    {
        var acc = Domain.Aggregates.Account.Create(AccountId.Of(account.Id), AccountNumber.Of(account.AccountNumber),
            account.Amount, account.OpenDate);
        acc.IsDeleted = account.IsDeleted;
        return acc;
    }
    
    public static Domain.Aggregates.Customer CustomerFromDb(Customer customer)
    {
        var cust = Domain.Aggregates.Customer.CreateWithAccounts(CustomerId.Of(customer.Id), customer.Name,
            customer.Accounts.Select(AccountFromDb).ToList());
        cust.IsDeleted = customer.IsDeleted;
        return cust;
    }
    
    public static Domain.Aggregates.Transaction TransactionFromDb(Transaction transaction)
    {
        var senderAcc = AccountFromDb(transaction.SenderAccount);
        var recipAcc = AccountFromDb(transaction.RecipientAccount);
        var tr = Domain.Aggregates.Transaction.Create(senderAcc, recipAcc, transaction.Amount, dateTime: transaction.CreatedDate);
        tr.Id = TransactionId.Of(transaction.Id);
        return tr;
    }
}