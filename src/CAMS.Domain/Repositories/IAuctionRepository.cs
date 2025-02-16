using CAMS.Domain.Entities;

namespace CAMS.Domain.Repositories;

/// <summary>
/// Represents an auction repository.
/// </summary>
public interface IAuctionRepository
{
    Task AddAsync(Auction auction);
    Task<Auction?> GetActiveAuctionByVehicleIdAsync(Guid vehicleId);
    Task<Auction?> GetByIdAsync(Guid auctionId);
    Task UpdateAsync(Auction auction);
    Task<IEnumerable<Auction>> SearchAsync(Func<Auction, bool> predicate);
}
