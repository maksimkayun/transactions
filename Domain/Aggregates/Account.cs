using Domain.Aggregates.Common;
using Domain.Aggregates.Events;
using Domain.Aggregates.Exceptions;
using IAggregate = Domain.Common.IAggregate;

namespace Domain.Aggregates;

public class Account : Aggregate<AccountId>, IAggregate
{
    public AccountNumber Number { get; private set; } = default!;
    
    public List<Transaction> IncomingTransactions { get; private set; } = default!;
    public List<Transaction> OutgoingTransactions { get; private set; } = default!;
    
    public DateTime OpenDate { get; private set; }

    public static Account Create(AccountId accountId, AccountNumber accountNumber, decimal amount = 0, DateTime? openDate = null)
    {
        var acc = new Account
        {
            IsDeleted = false,
            Amount = amount >= 0 ? amount : throw new IncorrectStartAmountException("The initial amount cannot be less than 0!"),
            Number = accountNumber,
            Id = accountId,
            IncomingTransactions = new List<Transaction>(),
            OutgoingTransactions = new List<Transaction>(),
            OpenDate = openDate ?? DateTime.UtcNow
        };

        var @event = new AccountCreatedDomainEvent(acc);
        acc.AddDomainEvent(@event);

        return acc;
    }
    
    public Account ReopenAccount(decimal startAmount = 0)
    {
        IsDeleted = false;
        OpenDate = DateTime.UtcNow;
        if (startAmount >= 0)
        {
            Amount = startAmount;
        }
        else
        {
            throw new IncorrectStartAmountException("The initial amount cannot be less than 0!");
        }
        return this;
    }

    public Account CloseAccount()
    {
        IsDeleted = true;
        var @event = new AccountClosedDomainEvent(this);
        
        this.AddDomainEvent(@event);

        return this;
    }
    
    public decimal Amount { get; private set; }

    public decimal Increase(decimal value)
    {
        if (value > 0)
        {
            Amount += value;
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
            Amount -= value;
        }
        else
        {
            throw new InvalidDecreaseException(value <=  0 ? "Сумма для списания должна быть больше 0!" : "Недостаточно средств для списания!");
        }

        return Amount;
    }
}