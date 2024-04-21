using Domain.Common;

namespace Domain.Aggregates.Events;

public record ChangeCustomerDomainEvent(Customer Customer) : IDomainEvent;