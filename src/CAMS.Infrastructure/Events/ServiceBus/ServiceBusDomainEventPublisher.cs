using CAMS.Domain.Entities;
using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events.ServiceBus;

/// <summary>
/// Publishes domain events to Azure Service Bus using an injected service bus publisher.
/// </summary>
public class ServiceBusDomainEventPublisher : IDomainEventPublisher
    {
        private readonly IServiceBusPublisher _serviceBusPublisher;
        private readonly string _destinationQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusDomainEventPublisher"/> class.
        /// </summary>
        /// <param name="serviceBusPublisher">The service bus publisher to send messages.</param>
        /// <param name="destinationQueue">The name of the destination queue.</param>
        public ServiceBusDomainEventPublisher(IServiceBusPublisher serviceBusPublisher, string destinationQueue)
        {
            _serviceBusPublisher = serviceBusPublisher ?? throw new ArgumentNullException(nameof(serviceBusPublisher));
            _destinationQueue = destinationQueue ?? throw new ArgumentNullException(nameof(destinationQueue));
        }

        /// <summary>
        /// Publishes all domain events from the given aggregate to the configured Service Bus queue,
        /// and then clears the domain events.
        /// </summary>
        /// <typeparam name="T">The type of the aggregate that implements IHasDomainEvents.</typeparam>
        /// <param name="aggregate">The aggregate containing domain events.</param>
        public async Task PublishEventsAsync<T>(T aggregate) where T : IHasDomainEvents
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                // Use the aggregate's ID (assuming it has one) as the session id.                
                string sessionId = (aggregate as Entity)?.Id.ToString() ?? Guid.NewGuid().ToString();
                await _serviceBusPublisher.PublishToQueueAsync(_destinationQueue, domainEvent.ToJson(), sessionId);
            }

            aggregate.DomainEvents.Clear();
        }
}

