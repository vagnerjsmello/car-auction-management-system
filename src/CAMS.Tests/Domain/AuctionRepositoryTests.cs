using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Repositories;
using FluentAssertions;

namespace CAMS.Tests.Domain
{
    /// <summary>
    /// Unit tests for the in-memory implementation of IAuctionRepository.
    /// </summary>
    public class AuctionRepositoryTests
    {
        private readonly IAuctionRepository _repository;

        public AuctionRepositoryTests()
        {
            _repository = new InMemoryAuctionRepository();
        }

        [Fact]
        public async Task AddAsync_Should_AddAuctionSuccessfully()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var auction = new Auction(vehicleId, 10000m);

            // Act
            await _repository.AddAsync(auction);
            var retrievedAuction = await _repository.GetActiveAuctionByVehicleIdAsync(vehicleId);

            // Assert
            retrievedAuction.Should().NotBeNull();
            retrievedAuction.Id.Should().Be(auction.Id);
            retrievedAuction.VehicleId.Should().Be(vehicleId);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenAuctionAlreadyExistsForVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var auction1 = new Auction(vehicleId, 10000m);
            var auction2 = new Auction(vehicleId, 15000m);

            await _repository.AddAsync(auction1);

            // Act
            Func<Task> act = async () => await _repository.AddAsync(auction2);

            // Assert
            await act.Should().ThrowAsync<AuctionAlreadyActiveException>()
                .WithMessage($"*{vehicleId}*");
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnCorrectAuction()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var auction = new Auction(vehicleId, 10000m);
            await _repository.AddAsync(auction);

            // Act
            var retrievedAuction = await _repository.GetByIdAsync(auction.Id);

            // Assert
            retrievedAuction.Should().NotBeNull();
            retrievedAuction.Id.Should().Be(auction.Id);
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateAuctionStatus()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var auction = new Auction(vehicleId, 10000m);
            await _repository.AddAsync(auction);

            // Act
            auction.Close();
            await _repository.UpdateAsync(auction);
            var updatedAuction = await _repository.GetActiveAuctionByVehicleIdAsync(vehicleId);

            // Assert
            updatedAuction.Should().NotBeNull();
            updatedAuction.Status.Should().Be(AuctionStatus.Closed);
        }

        [Fact]
        public async Task SearchAsync_Should_ReturnAuctionsMatchingPredicate()
        {
            // Arrange
            var vehicleId1 = Guid.NewGuid();
            var vehicleId2 = Guid.NewGuid();
            var auction1 = new Auction(vehicleId1, 10000m);
            var auction2 = new Auction(vehicleId2, 15000m);
            await _repository.AddAsync(auction1);
            await _repository.AddAsync(auction2);

            // Act
            var results = await _repository.SearchAsync(a => a.HighestBid >= 15000m);

            // Assert
            results.Should().ContainSingle().Which.Should().Be(auction2);
        }
    }
}
