using CAMS.Domain.Events;

namespace CAMS.Infrastructure.Events;

/// <summary>
/// A fake implementation of IDomainEventDispatcher that simulates event dispatching.
/// </summary>
public class FakeDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(DomainEvent domainEvent)
    {
        Console.WriteLine($"Domain event dispatched: {domainEvent.GetType().Name}");
        Console.WriteLine(domainEvent.ToJson());
        return Task.CompletedTask;
    }
}