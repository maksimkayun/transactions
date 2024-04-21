using Domain.Common;

namespace Domain.Aggregates.Events;

public record CustomerCreatedDomainEvent(Customer Customer) : IDomainEvent;