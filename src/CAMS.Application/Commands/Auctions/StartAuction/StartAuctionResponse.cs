namespace CAMS.Application.Commands.Auctions.StartAuction;


/// <summary>
/// Response returned after starting an auction.
/// </summary>
public class StartAuctionResponse
{
    public Guid AuctionId { get; }
    public Guid VehicleId { get; }
    public decimal StartingBid { get; }

    public StartAuctionResponse(Guid auctionId, Guid vehicleId, decimal startingBid)
    {
        AuctionId = auctionId;
        VehicleId = vehicleId;
        StartingBid = startingBid;
    }
}