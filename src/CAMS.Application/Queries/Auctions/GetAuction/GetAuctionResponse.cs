using CAMS.Domain.Enums;

namespace CAMS.Application.Queries.Auctions.GetAuction;

/// <summary>
/// Response containing auction details.
/// </summary>
public class GetAuctionResponse
{
    public Guid AuctionId { get; }
    public Guid VehicleId { get; }
    public decimal HighestBid { get; }
    public AuctionStatus Status { get; }

    public GetAuctionResponse(Guid auctionId, Guid vehicleId, decimal highestBid, AuctionStatus status)
    {
        AuctionId = auctionId;
        VehicleId = vehicleId;
        HighestBid = highestBid;
        Status = status;
    }
}
