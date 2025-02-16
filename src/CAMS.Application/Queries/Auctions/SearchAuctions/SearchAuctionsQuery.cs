using CAMS.Domain.Enums;
using MediatR;

namespace CAMS.Application.Queries.Auctions.SearchAuctions;

/// <summary>
/// Query to search auctions based on filter criteria.
/// </summary>
public class SearchAuctionsQuery : IRequest<SearchAuctionsResponse>
{
    public AuctionStatus? Status { get; }
    public Guid? VehicleId { get; }

    public SearchAuctionsQuery(AuctionStatus? status, Guid? vehicleId)
    {
        Status = status;
        VehicleId = vehicleId;
    }
}
