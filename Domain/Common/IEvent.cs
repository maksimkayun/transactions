using MassTransit;
using MediatR;

namespace Domain.Common;

public interface IEvent: INotification
{
    // Лучше - https://github.com/phatboyg/NewId
    Guid EventId => NewId.NextGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}