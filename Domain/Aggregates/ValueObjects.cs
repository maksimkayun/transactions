using Domain.Aggregates.Common;
using Domain.Aggregates.Exceptions;
using MassTransit;

namespace Domain.Aggregates;

public class TransactionStatus : Enumeration
{
    public static TransactionStatus Created = new(1, nameof(Created), "Транзакция создана");
    public static TransactionStatus Processing = new(2, nameof(Processing), "Транзакция в обработке");
    public static TransactionStatus Completed = new(3, nameof(Completed), "Транзакция обработана");
    public static TransactionStatus Cancelled = new(4, nameof(Cancelled), "Транзакция отменена");

    public TransactionStatus() : base(Created.Id, Created.Name)
    {}
    public void WithReason(string descr)
    {
        if (Equals(this, Cancelled))
        {
            Description = descr;
        }
    }
    
    public string Description { get; private set; }
    public TransactionStatus(int id, string name, string description) : base(id, name)
    {
        Description = description;
    }
}
public class AccountId
{
    public Guid Value { get; }

    private AccountId(Guid value)
    {
        Value = value;
    }

    public static AccountId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAccountIdException(value);
        }

        return new AccountId(value);
    }
    
    public static AccountId Of(string value)
    {
        if (Guid.TryParse(value, out var val) && val != Guid.Empty)
        {
            return new AccountId(val); 
        }
        
        throw new InvalidAccountIdException(val);
    }

    public static implicit operator Guid(AccountId airportId)
    {
        return airportId.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class TransactionResult : ValueObject<TransactionResult>
{
    private decimal _senderRemains;
    private decimal _recipientRemains;
    public TransactionStatus TransactionStatus { get; private set; }
    
    public string Message { get; private set; }

    public TransactionResult(decimal senderRemains, decimal recipientRemains, TransactionStatus status)
    {
        _senderRemains = senderRemains >= 0 && !status.Equals(TransactionStatus.Cancelled) ? senderRemains : throw new Exception("Некорректная сумма остатка у отправителя");
        _recipientRemains = recipientRemains >= 0 && !status.Equals(TransactionStatus.Cancelled) ? recipientRemains : throw new Exception("Некорректная сумма остатка у получателя");
        TransactionStatus = status;
    }

    public void UpdateStatus(TransactionStatus status)
    {
        TransactionStatus = status;
    }

    protected override bool EqualsCore(TransactionResult other)
    {
        return GetHashCodeCore() == other.GetHashCodeCore();
    }

    protected override int GetHashCodeCore()
    {
        return _senderRemains.GetHashCode() + _recipientRemains.GetHashCode() + TransactionStatus.GetHashCode();
    }

    public TransactionResult WithCheck()
    {
        if (TransactionStatus.Equals(TransactionStatus.Completed) || TransactionStatus.Equals(TransactionStatus.Cancelled))
        {
            Message = string.Empty;
        }
        else
        {
            Message = "Транзакция ещё в работе";
        }
        
        return this;
    }
}



public class TransactionId
{
    public Guid Value { get; }

    private TransactionId(Guid value)
    {
        Value = value;
    }

    public static TransactionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidTransactionIdException(value);
        }

        return new TransactionId(value);
    }
    public static TransactionId Of(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var val) && val != Guid.Empty)
        {
            return new TransactionId(val);
        }
        
        throw new InvalidTransactionIdException(value);
    }

    public static implicit operator Guid(TransactionId airportId)
    {
        return airportId.Value;
    }
}

public class CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value)
    {
        Value = value;
    }

    public static CustomerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidCustomerIdException(value);
        }

        return new CustomerId(value);
    }
    
    public static CustomerId Of(string value)
    {
        if (!string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var val) && val != Guid.Empty)
        {
            return new CustomerId(val);
        }
        
        throw new InvalidCustomerIdException(value);
    }

    public static implicit operator Guid(CustomerId customerId)
    {
        return customerId.Value;
    }
}

public class AccountNumber
{
    public static long START_VALUE = 7770000;
    public long Value { get; }

    private AccountNumber(long value)
    {
        Value = value;
    }

    public static AccountNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidAccountNumberException();
        }

        if (long.TryParse(value, out var val))
        {
            return new AccountNumber(val);
        }

        throw new InvalidAccountNumberFormatException(badValue: value);
    }
    
    public static AccountNumber Of(long value)
    {
        if (value >= START_VALUE)
        {
            return new AccountNumber(value);
        }
        throw new InvalidAccountNumberFormatException(badValue: value.ToString());
    }
    
    public static implicit operator string(AccountNumber name)
    {
        return name.Value.ToString();
    }
}

