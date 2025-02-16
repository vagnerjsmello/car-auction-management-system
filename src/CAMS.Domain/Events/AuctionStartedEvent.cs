namespace CAMS.Domain.Events;

/// <summary>
/// Event raised when an auction is started.
/// </summary>
public class AuctionStartedEvent : DomainEvent
{
    public Guid AuctionId { get; }
    public Guid VehicleId { get; }
    public decimal StartingBid { get; }

    public AuctionStartedEvent(Guid auctionId, Guid vehicleId, decimal startingBid)
    {
        AuctionId = auctionId;
        VehicleId = vehicleId;
        StartingBid = startingBid;
    }
}
