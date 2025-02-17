using CAMS.Domain.Entities;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using System.Collections.Concurrent;

namespace CAMS.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of IAuctionRepository.
/// </summary>
public class InMemoryAuctionRepository : IAuctionRepository
{
    private readonly ConcurrentDictionary<Guid, Auction> _auctions = new ConcurrentDictionary<Guid, Auction>();

    public async Task AddAsync(Auction auction)
    {        
        if (!_auctions.TryAdd(auction.VehicleId, auction))
        {
            throw new AuctionAlreadyActiveException(auction.VehicleId);
        }
        await Task.CompletedTask;
    }

    public async Task<Auction?> GetActiveAuctionByVehicleIdAsync(Guid vehicleId)
    {
        _auctions.TryGetValue(vehicleId, out var auction);
        return await Task.FromResult(auction);
    }

    public async Task<Auction?> GetByIdAsync(Guid auctionId)
    {        
        var auction = _auctions.Values.FirstOrDefault(a => a.Id == auctionId);
        return await Task.FromResult(auction);
    }

    public async Task UpdateAsync(Auction auction)
    {
        _auctions.AddOrUpdate(auction.VehicleId, auction, (key, existing) => auction);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Auction>> SearchAsync(Func<Auction, bool> predicate)
    {
        var result = _auctions.Values.Where(predicate);
        return await Task.FromResult(result);
    }
}

