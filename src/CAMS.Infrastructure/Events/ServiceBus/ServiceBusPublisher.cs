using Azure.Messaging.ServiceBus;
using System.Configuration;

namespace CAMS.Infrastructure.Events.ServiceBus;

public class ServiceBusPublisher : IServiceBusPublisher, IDisposable
{
    private readonly ServiceBusClient _client;

    /// <summary>
    /// Creates a new instance of ServiceBusPublisher.
    /// </summary>
    public ServiceBusPublisher()
    {
        string connectionString = // Retrieve from the configuration file.
            ConfigurationManager.ConnectionStrings["ServiceBusConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("Service Bus connection string is not defined in the configuration.");

        // The ServiceBusClient uses built-in retry policies by default.
        _client = new ServiceBusClient(connectionString);
    }

    public async Task PublishToQueueAsync(string queueName, string messageBody, string sessionId = null)
    {
        ServiceBusSender sender = _client.CreateSender(queueName);
        ServiceBusMessage message = new ServiceBusMessage(messageBody)
        {
            SessionId = sessionId,
            MessageId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(message);
        await sender.DisposeAsync();
    }

    public async Task PublishToTopicAsync(string topicName, string messageBody, string sessionId = null)
    {
        ServiceBusSender sender = _client.CreateSender(topicName);
        ServiceBusMessage message = new ServiceBusMessage(messageBody)
        {
            SessionId = sessionId,
            MessageId = Guid.NewGuid().ToString()
        };

        await sender.SendMessageAsync(message);
        await sender.DisposeAsync();
    }

    public void Dispose()
    {
        _client.DisposeAsync().AsTask().Wait();
    }
}


