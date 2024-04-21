using Domain.Common;

namespace Domain.Aggregates.Events;

public record TransactionCreatedDomainEvent(Transaction Transaction) : IDomainEvent;