using Domain.Aggregates.Common;
using Domain.Aggregates.Events;
using MassTransit;

namespace Domain.Aggregates;

public class Customer : Aggregate<CustomerId>
{
    public bool IsDeleted { get; private set; } = default!;
    public string Name { get; private set; } = default!;

    private List<Account> _accounts;
    
    public IReadOnlyCollection<Account> Accounts => _accounts;

    public static Customer Create(CustomerId customerId, string name)
    {
        var cust = new Customer
        {
            Id = customerId,
            Name = name,
            IsDeleted = false,
            _accounts = new List<Account>()
        };

        var @event = new CustomerCreatedDomainEvent(cust);
        cust.AddDomainEvent(@event);

        return cust;
    }

    public Account OpenAccount(long accNumber, decimal amount = 0)
    {
        var newAccount = Account.Create(AccountId.Of(NewId.NextGuid()), accountNumber: AccountNumber.Of(accNumber), amount);
        if (_accounts == null)
        {
            _accounts = new List<Account>();
        }
        _accounts.Add(newAccount);
        var @event = new ChangeCustomerDomainEvent(this);
        this.AddDomainEvent(@event);
        
        return newAccount;
    }

    public Account? CloseAccount(long number)
    {
        var acc = _accounts.FirstOrDefault(e => e.Number == number.ToString());
        if (acc != null)
        {
            _accounts.Remove(acc);
        }

        return acc;
    }


}