namespace Transactions.Aggregates.Common;

public class Entity<T>: IEntity<T>
{
    public T Id { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    
}