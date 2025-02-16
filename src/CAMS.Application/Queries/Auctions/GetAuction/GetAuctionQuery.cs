using MediatR;

namespace CAMS.Application.Queries.Auctions.GetAuction;

/// <summary>
/// Query to retrieve details of a specific auction.
/// </summary>
public class GetAuctionQuery : IRequest<GetAuctionResponse>
{
    public Guid AuctionId { get; }

    public GetAuctionQuery(Guid auctionId)
    {
        AuctionId = auctionId;
    }
}
