using System.Text.Json;

namespace CAMS.Domain.Events;


/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public virtual string ToJson() => JsonSerializer.Serialize(this);
}