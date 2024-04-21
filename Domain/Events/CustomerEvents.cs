using Domain.Aggregates;
using MediatR;

namespace Domain.Events;

public record class CustomerCreated(Customer Transaction) : INotification;
public record class CustomerChanged(Customer Transaction) : INotification;