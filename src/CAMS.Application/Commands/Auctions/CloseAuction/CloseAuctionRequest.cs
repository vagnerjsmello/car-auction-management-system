namespace CAMS.Application.Commands.Auctions.CloseAuction;

/// <summary>
/// DTO that encapsulates the data needed to close an auction.
/// </summary>
public class CloseAuctionRequest
{
    public Guid AuctionId { get; set; }
}
