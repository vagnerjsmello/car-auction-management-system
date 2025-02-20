namespace CAMS.Infrastructure.Events.ServiceBus;

/// <summary>
/// Defines a contract for publishing messages to Azure Service Bus.
/// </summary>
public interface IServiceBusPublisher
{
    /// <summary>
    /// Publishes a message to the specified queue.
    /// </summary>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="messageBody">The message content.</param>
    /// <param name="sessionId">Optional session identifier for ordered processing.</param>
    Task PublishToQueueAsync(string queueName, string messageBody, string sessionId = null);

    /// <summary>
    /// Publishes a message to the specified topic.
    /// </summary>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="messageBody">The message content.</param>
    /// <param name="sessionId">Optional session identifier for ordered processing.</param>
    Task PublishToTopicAsync(string topicName, string messageBody, string sessionId = null);
}
