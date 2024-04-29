
using Domain.Common;

namespace Domain.Aggregates.Common;

public class Entity<T>: IEntity<T>
{
    public T Id { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class Aggregate<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public IEvent[] ClearDomainEvents()
    {
        IEvent[] dequeuedEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeuedEvents;
    }
}