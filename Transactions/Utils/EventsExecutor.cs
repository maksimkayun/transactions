using Domain.Common;
using MediatR;

namespace Transactions.Utils;

public class EventsExecutor
{
    private IMediator _mediator;

    public EventsExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExecuteEvents(IAggregate aggregate, CancellationToken cancellationToken = new CancellationToken())
    {
        if (aggregate.GetDomainEvents() != null && aggregate.GetDomainEvents().Any())
        {
            var events = aggregate.GetDomainEvents().ToList();
            aggregate.ClearDomainEvents();

            foreach (var ev in events)
            {
                await _mediator.Publish(ev, cancellationToken);
            }
        }
    }
}