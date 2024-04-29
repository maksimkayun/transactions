using Domain.Aggregates;
using MediatR;

namespace Transactions.Features;

public record class TransactionCreated(Transaction Transaction) : INotification;
public record class TransactionProcessed(TransactionResult Transaction) : INotification;
public record class TransactionCancelled(TransactionResult Transaction) : INotification;