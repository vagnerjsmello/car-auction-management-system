using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events.FakePublisher;


/// <summary>
/// Defines a contract for dispatching domain events.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent);
}
