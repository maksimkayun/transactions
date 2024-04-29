using Domain.Aggregates;
using Domain.Aggregates.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Transactions.DataAccess;
using Transactions.Features;

namespace Transactions.DomainEventHandlers;

public class TransactionCreatedHandler : INotificationHandler<TransactionCreatedDomainEvent>
{
    private IMediator _mediator;
    private TransactionsContext _context;

    public TransactionCreatedHandler(IMediator mediator, TransactionsContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task Handle(TransactionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
    }
}