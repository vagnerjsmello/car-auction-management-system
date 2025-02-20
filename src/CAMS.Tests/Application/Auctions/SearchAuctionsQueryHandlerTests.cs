using CAMS.Application.Queries.Auctions.SearchAuctions;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Auctions;

/// <summary>
/// Unit tests for the SearchAuctionsQueryHandler.
/// </summary>
public class SearchAuctionsQueryHandlerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<ILogger<SearchAuctionsQueryHandler>> _loggerMock;
    private readonly SearchAuctionsQueryHandler _handler;

    public SearchAuctionsQueryHandlerTests()
    {
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _loggerMock = new Mock<ILogger<SearchAuctionsQueryHandler>>();
        _handler = new SearchAuctionsQueryHandler(_auctionRepositoryMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Act
        Action act = () => new SearchAuctionsQueryHandler(null!, _loggerMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("auctionRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act
        Action act = () => new SearchAuctionsQueryHandler(_auctionRepositoryMock.Object, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new SearchAuctionsQueryHandler(_auctionRepositoryMock.Object, _loggerMock.Object);
        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests

    [Fact]
    public async Task Handle_ShouldReturnAuctionsMatchingStatusFilter()
    {
        // Arrange
        var auction1 = new Auction(Guid.NewGuid(), 10000m);

        var auction2 = new Auction(Guid.NewGuid(), 15000m);
        auction2.Close(); // Set auction2 as Closed.

        var auctions = new List<Auction> { auction1, auction2 };
        _auctionRepositoryMock.Setup(r => r.SearchAsync(It.IsAny<Func<Auction, bool>>()))
                              .ReturnsAsync((Func<Auction, bool> predicate) => auctions.Where(predicate));

        var query = new SearchAuctionsQuery(AuctionStatus.Active, null);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);
        // Assert
        response.Data.Auctions.Should().ContainSingle(a => a.Id == auction1.Id);
        response.Data.Auctions.Should().NotContain(a => a.Id == auction2.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuctionsMatchingVehicleIdFilter()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var auction1 = new Auction(vehicleId, 10000m);
        var auction2 = new Auction(Guid.NewGuid(), 15000m);

        var auctions = new List<Auction> { auction1, auction2 };
        _auctionRepositoryMock.Setup(r => r.SearchAsync(It.IsAny<Func<Auction, bool>>()))
                              .ReturnsAsync((Func<Auction, bool> predicate) => auctions.Where(predicate));

        var query = new SearchAuctionsQuery(null, vehicleId);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Auctions.Should().ContainSingle(a => a.VehicleId == vehicleId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoAuctionsMatchFilter()
    {
        // Arrange
        var auctions = new List<Auction>
        {
            new Auction(Guid.NewGuid(), 10000m),
            new Auction(Guid.NewGuid(), 15000m)
        };
        _auctionRepositoryMock.Setup(r => r.SearchAsync(It.IsAny<Func<Auction, bool>>()))
                              .ReturnsAsync((Func<Auction, bool> predicate) => auctions.Where(predicate));

        var query = new SearchAuctionsQuery(null, Guid.NewGuid());

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Auctions.Should().BeEmpty();
    }


    [Fact]
    public async Task Handle_ShouldReturnAuctionsMatchingCombinedFilters()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var auctionMatching = new Auction(vehicleId, 10000m);
        auctionMatching.Close();

        var auctionNonMatching = new Auction(Guid.NewGuid(), 15000m);
        var auctions = new List<Auction> { auctionMatching, auctionNonMatching };

        _auctionRepositoryMock.Setup(r => r.SearchAsync(It.IsAny<Func<Auction, bool>>()))
                              .ReturnsAsync((Func<Auction, bool> predicate) => auctions.Where(predicate));

        var query = new SearchAuctionsQuery(AuctionStatus.Closed, vehicleId);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Data.Auctions.Should().ContainSingle(a => a.Id == auctionMatching.Id);
        response.Data.Auctions.Should().NotContain(a => a.Id == auctionNonMatching.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllAuctions_WhenNoFiltersProvided()
    {
        // Arrange
        var auction1 = new Auction(Guid.NewGuid(), 10000m);
        var auction2 = new Auction(Guid.NewGuid(), 15000m);
        var auctions = new List<Auction> { auction1, auction2 };

        _auctionRepositoryMock.Setup(r => r.SearchAsync(It.IsAny<Func<Auction, bool>>()))
                              .ReturnsAsync((Func<Auction, bool> predicate) => auctions.Where(predicate));

        var query = new SearchAuctionsQuery(null, null);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Data.Auctions.Should().HaveCount(2);
    }
    #endregion
}
