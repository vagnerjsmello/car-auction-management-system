using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events;

/// <summary>
/// Defines a contract for publishing domain events.
/// </summary>
public interface IDomainEventPublisher
{
    Task PublishEventsAsync<T>(T aggregate) where T : IHasDomainEvents;
}
