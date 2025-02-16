using CAMS.Domain.Events;

namespace CAMS.Domain.Entities;

/// <summary>
/// Base class for aggregate roots that raise domain events.
/// </summary>
public abstract class AggregateRoot : Entity, IHasDomainEvents
{
    public List<DomainEvent> DomainEvents { get; } = new List<DomainEvent>();

    protected AggregateRoot(Guid id) : base(id)
    {
    }
}
