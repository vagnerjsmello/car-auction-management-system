using CAMS.Domain.Entities;
using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events.ServiceBus;

/// <summary>
/// Publishes domain events to an Azure Service Bus topic using an injected service bus publisher.
/// </summary>
public class ServiceBusDomainEventTopicPublisher : IDomainEventPublisher
{
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly string _destinationTopic;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusDomainEventTopicPublisher"/> class.
    /// </summary>
    /// <param name="serviceBusPublisher">The service bus publisher to send messages.</param>
    /// <param name="destinationTopic">The name of the destination topic.</param>
    public ServiceBusDomainEventTopicPublisher(IServiceBusPublisher serviceBusPublisher, string destinationTopic)
    {
        _serviceBusPublisher = serviceBusPublisher ?? throw new ArgumentNullException(nameof(serviceBusPublisher));
        _destinationTopic = destinationTopic ?? throw new ArgumentNullException(nameof(destinationTopic));
    }

    /// <summary>
    /// Publishes all domain events from the given aggregate to the configured Service Bus topic,
    /// and then clears the domain events.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate that implements IHasDomainEvents.</typeparam>
    /// <param name="aggregate">The aggregate containing domain events.</param>
    public async Task PublishEventsAsync<T>(T aggregate) where T : IHasDomainEvents
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            // Use the aggregate's ID as the session id to maintain ordering.
            string sessionId = (aggregate as Entity)?.Id.ToString() ?? Guid.NewGuid().ToString();
            await _serviceBusPublisher.PublishToTopicAsync(_destinationTopic, domainEvent.ToJson(), sessionId);
        }
        aggregate.DomainEvents.Clear();
    }
}

