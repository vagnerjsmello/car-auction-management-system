using CAMS.Application.Common;
using MediatR;

namespace CAMS.Application.Commands.Auctions.CloseAuction;

/// <summary>
/// Command to close an active auction.
/// </summary>
public class CloseAuctionCommand : IRequest<OperationResult<CloseAuctionResponse>>
{
    public Guid AuctionId { get; }

    public CloseAuctionCommand(CloseAuctionRequest request)
    {
        AuctionId = request.AuctionId;
    }
}
