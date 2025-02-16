namespace CAMS.Domain.Events;

/// <summary>
/// Event raised when an auction is closed.
/// </summary>
public class AuctionClosedEvent : DomainEvent
{
    public Guid AuctionId { get; }
    public Guid VehicleId { get; }  
    public decimal FinalHighestBid { get; }
    public DateTime ClosedAt { get; }

    public AuctionClosedEvent(Guid auctionId, Guid vehicleId, decimal finalHighestBid, DateTime closedAt)
    {
        AuctionId = auctionId;
        VehicleId = vehicleId;
        FinalHighestBid = finalHighestBid;
        ClosedAt = closedAt;
    }
}
