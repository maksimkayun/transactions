using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class Transaction : IAggregate<Transaction>
{
    public bool IsDeleted { get; set; }
    public Transaction Id { get; set; }
}