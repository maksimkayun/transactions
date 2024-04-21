using Domain.Common;

namespace Domain.Aggregates.Events;

public record AccountCreatedDomainEvent(Account Account) : IDomainEvent;