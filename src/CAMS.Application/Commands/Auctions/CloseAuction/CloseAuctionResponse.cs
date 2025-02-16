namespace CAMS.Application.Commands.Auctions.CloseAuction;

/// <summary>
/// Response returned after closing an auction.
/// </summary>
public class CloseAuctionResponse
{
    public Guid AuctionId { get; }

    public CloseAuctionResponse(Guid auctionId)
    {
        AuctionId = auctionId;
    }
}
