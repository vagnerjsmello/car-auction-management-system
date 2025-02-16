using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events;


/// <summary>
/// Defines a contract for dispatching domain events.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(DomainEvent domainEvent);
}
