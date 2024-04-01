using System.Collections;
using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class TransactionStatus : Enumeration
{
    public static TransactionStatus Created = new(1, nameof(Created), "Транзакция создана");
    public static TransactionStatus Processing = new(2, nameof(Processing), "Транзакция в обработке");
    public static TransactionStatus Completed = new(3, nameof(Completed), "Транзакция обработана");
    public static TransactionStatus Cancelled = new(4, nameof(Cancelled), "Транзакция отменена");

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


    public IEnumerator GetEnumerator()
    {
        yield return Created;
        yield return Processing;
        yield return Completed;
        yield return Cancelled;
    }
}

public class AccountNumber(string id, string number, decimal amount) : ValueObject<AccountNumber>
{
    private decimal _amount = amount > 0 ? amount : throw new Exception("Сумма для списания/начисления должна быть больше 0!");
    public string Number { get; private set; } = number;
    public string Id { get; private set; } = id;

    public decimal GetAmount() => _amount;

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

        return GetAmount();
    }
    
    public decimal Decrease(decimal value)
    {
        if (value > 0 && GetAmount() - value >= 0)
        {
            _amount -= value;
        }
        else
        {
            throw new Exception(value <=  0 ? "Сумма для списания должна быть больше 0!" : "Недостаточно средств для списания!");
        }

        return GetAmount();
    }

    protected override bool EqualsCore(AccountNumber other)
        => this.GetHashCodeCore() == other.GetHashCodeCore();

    protected override int GetHashCodeCore() => Id.GetHashCode() + Number.GetHashCode();
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
