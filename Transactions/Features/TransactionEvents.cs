using Domain.Aggregates;
using MediatR;

namespace Domain.Events;

public record class TransactionCreated(Transaction Transaction) : INotification;
public record class TransactionProcessed(TransactionResult Transaction) : INotification;
public record class TransactionCancelled(TransactionResult Transaction) : INotification;