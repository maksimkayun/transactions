namespace Domain.Common;

public interface IAggregate
{
    void AddDomainEvent(IDomainEvent domainEvent);
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    IEvent[] ClearDomainEvents();
}