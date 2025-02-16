namespace CAMS.Application.Commands.Auctions.StartAuction;

/// <summary>
/// DTO that encapsulates the data needed to start an auction.
/// </summary>
public class StartAuctionRequest
{
    public Guid VehicleId { get; set; }
    public decimal StartingBid { get; set; }
}
