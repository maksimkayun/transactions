using Domain.Aggregates.Common;
using Domain.Aggregates.Events;

namespace Domain.Aggregates;

public class Account : Aggregate<AccountId>
{
    private decimal _amount;
    public AccountNumber Number { get; private set; } = default!;
    
    public List<Transaction> IncomingTransactions { get; private set; } = default!;
    public List<Transaction> OutgoingTransactions { get; private set; } = default!;

    public static Account Create(AccountId accountId, AccountNumber accountNumber, decimal amount = 0)
    {
        var acc = new Account
        {
            IsDeleted = false,
            _amount = amount > 0 ? amount : 0,
            Number = accountNumber,
            Id = accountId,
            IncomingTransactions = new List<Transaction>(),
            OutgoingTransactions = new List<Transaction>()
        };

        var @event = new AccountCreatedDomainEvent(acc);
        acc.AddDomainEvent(@event);

        return acc;
    }

    public Account CloseAccount()
    {
        IsDeleted = true;
        var @event = new AccountClosedDomainEvent(this);
        
        this.AddDomainEvent(@event);

        return this;
    }
    
    public decimal Amount => _amount;

    public decimal Increase(decimal value)
    {
        if (value > 0)
        {
            _amount += value;
        }
        else
        {
            throw new Exception("Сумма для начисления должна быть больше 0!");
        }

        return Amount;
    }
    
    public decimal Decrease(decimal value)
    {
        if (value > 0 && Amount - value >= 0)
        {
            _amount -= value;
        }
        else
        {
            throw new Exception(value <=  0 ? "Сумма для списания должна быть больше 0!" : "Недостаточно средств для списания!");
        }

        return Amount;
    }
}