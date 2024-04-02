using Microsoft.AspNetCore.Mvc;
using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class Customer : IAggregate<Customer>
{
    public bool IsDeleted { get; private set; }
    public Customer Id { get; private set; }
    public string CustomerId { get; }
    public string Name { get; }

    private readonly List<AccountNumber> _accounts;
    
    public IReadOnlyCollection<AccountNumber> Accounts => _accounts;

    public Customer(string name, string? customerId = null)
    {
        Id = this;
        IsDeleted = false;
        _accounts = [];
        CustomerId =  customerId ?? Guid.NewGuid().ToString();
        Name = name;
    }

    public AccountNumber OpenAccount(long accNumber, decimal amount = 0)
    {
        var newAccount = new AccountNumber(Guid.NewGuid().ToString(), accNumber.ToString(),
            amount > 0 ? amount : decimal.Zero);
        _accounts.Add(newAccount);
        return newAccount;
    }

    public AccountNumber? CloseAccount(long number)
    {
        var acc = _accounts.FirstOrDefault(e => e.Number == number.ToString());
        if (acc != null)
        {
            _accounts.Remove(acc);
        }

        return acc;
    }


}