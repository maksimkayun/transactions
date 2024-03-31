using Transactions.Aggregates.Common;

namespace Transactions.Aggregates;

public class Customer : IAggregate<Customer>
{
    public bool IsDeleted { get; set; }
    public Customer Id { get; set; }
}