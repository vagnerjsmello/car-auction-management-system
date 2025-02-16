namespace CAMS.Application.Commands.Auctions.PlaceBid;

/// <summary>
/// DTO that encapsulates the data needed to place a bid on an auction.
/// </summary>
public class PlaceBidRequest
{
    public Guid AuctionId { get; set; }
    public decimal BidAmount { get; set; }
    public Guid BidderId { get; set; }
}
