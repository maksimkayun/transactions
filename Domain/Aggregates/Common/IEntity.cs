namespace Domain.Aggregates.Common;

public interface IEntity<T> : IEntity
{
    public T Id { get; }
}

public interface IEntity
{
    public bool IsDeleted { get; }
}

// Aggregates Interfaces

public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
}
