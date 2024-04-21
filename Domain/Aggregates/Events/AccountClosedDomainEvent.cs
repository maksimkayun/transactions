using Domain.Common;

namespace Domain.Aggregates.Events;

public record AccountClosedDomainEvent(Account Account) : IDomainEvent;