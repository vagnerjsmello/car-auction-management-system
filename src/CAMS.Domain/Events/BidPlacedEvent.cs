namespace CAMS.Domain.Events;

/// <summary>
/// Event raised when a bid is placed on an auction.
/// </summary>
public class BidPlacedEvent : DomainEvent
{
    public Guid AuctionId { get; }
    public Guid VehicleId { get; }
    public decimal BidAmount { get; }
    public Guid BidderId { get; }
    public DateTime Timestamp { get; }

    public BidPlacedEvent(Guid auctionId, Guid vehicleId, decimal bidAmount, Guid bidderId, DateTime timestamp)
    {
        AuctionId = auctionId;
        VehicleId = vehicleId;
        BidAmount = bidAmount;
        BidderId = bidderId;
        Timestamp = timestamp;
    }
}
