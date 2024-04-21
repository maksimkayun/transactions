using Domain.Aggregates.Common;

namespace Domain.Aggregates;

public class Customer : Entity<Customer>
{
    public bool IsDeleted { get; private set; }
    public Customer Id { get; private set; }
    public string CustomerId { get; }
    public string Name { get; }

    private readonly List<AccountNumber> _accounts;
    
    public IReadOnlyCollection<AccountNumber> Accounts => _accounts;

    public Customer(string name, string? customerId = null, List<AccountNumber>? accounts = null)
    {
        Id = this;
        IsDeleted = false;
        _accounts = [];
        CustomerId =  customerId ?? Guid.NewGuid().ToString();
        Name = name;
        _accounts = accounts ?? new List<AccountNumber>();
    }

    public AccountNumber OpenAccount(long accNumber, decimal amount = 0)
    {
        var newAccount = AccountNumber.CreateNewAccountNumber(Guid.NewGuid().ToString(), accNumber.ToString(),
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