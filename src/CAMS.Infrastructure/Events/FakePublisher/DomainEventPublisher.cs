using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events.FakePublisher;

/// <summary>
/// Publishes domain events using an injected event dispatcher.
/// </summary>
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventPublisher(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task PublishEventsAsync<T>(T aggregate) where T : IHasDomainEvents
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            await _dispatcher.DispatchAsync(domainEvent);
        }
        aggregate.DomainEvents.Clear();
    }
}