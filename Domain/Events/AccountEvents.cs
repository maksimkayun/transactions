using Domain.Aggregates;
using MediatR;

namespace Domain.Events;

public record class AccountOpened(AccountNumber Transaction) : INotification;
public record class AccountClosed(AccountNumber Transaction) : INotification;
public record class AccountOwnerChanged(AccountNumber Transaction) : INotification;