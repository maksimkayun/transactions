using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class TransactionStatus : Enumeration
{
    public static TransactionStatus Created = new(1, nameof(Created), "Транзакция создана");
    public static TransactionStatus Processing = new(2, nameof(Processing), "Транзакция в обработке");
    public static TransactionStatus Completed = new(3, nameof(Completed), "Транзакция обработана");
    public static TransactionStatus Cancelled = new(4, nameof(Cancelled), "Транзакция отменена");
    
    public string Description { get; }
    public TransactionStatus(int id, string name, string description) : base(id, name)
    {
        Description = description;
    }
}