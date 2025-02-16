namespace CAMS.Application.Commands.Auctions.PlaceBid
{
    /// <summary>
    /// Response returned after placing a bid.
    /// </summary>
    public class PlaceBidResponse
    {
        public Guid AuctionId { get; }
        public decimal HighestBid { get; }

        public PlaceBidResponse(Guid auctionId, decimal highestBid)
        {
            AuctionId = auctionId;
            HighestBid = highestBid;
        }
    }
}
