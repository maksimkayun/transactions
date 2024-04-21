using Domain.Aggregates;
using Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Transactions.DomainEventHandlers;

public class TransactionCreatedHandler : INotificationHandler<TransactionCreated>
{
    private IMediator _mediator;
    private TransactionsContext _context;

    public TransactionCreatedHandler(IMediator mediator, TransactionsContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task Handle(TransactionCreated notification, CancellationToken cancellationToken)
    {
    }
}