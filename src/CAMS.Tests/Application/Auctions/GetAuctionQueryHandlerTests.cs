using CAMS.Application.Queries.Auctions.GetAuction;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Auctions;

/// <summary>
/// Unit tests for the GetAuctionQueryHandler.
/// </summary>
public class GetAuctionQueryHandlerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<ILogger<GetAuctionQueryHandler>> _loggerMock;
    private readonly GetAuctionQueryHandler _handler;

    public GetAuctionQueryHandlerTests()
    {
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _loggerMock = new Mock<ILogger<GetAuctionQueryHandler>>();
        _handler = new GetAuctionQueryHandler(_auctionRepositoryMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Act
        Action act = () => new GetAuctionQueryHandler(null!, _loggerMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("auctionRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act
        Action act = () => new GetAuctionQueryHandler(_auctionRepositoryMock.Object, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new GetAuctionQueryHandler(_auctionRepositoryMock.Object, _loggerMock.Object);
        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests

    [Fact]
    public async Task Handle_ShouldReturnAuctionDetails_WhenAuctionExists()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        decimal startingBid = 10000m;
        
        var auction = new Auction(vehicleId, startingBid);
        var auctionId = auction.Id;  // Use the generated auction Id in the query.
        auction.Status.Should().Be(AuctionStatus.Active);

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync(auction);

        var query = new GetAuctionQuery(auctionId);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.AuctionId.Should().Be(auction.Id);
        response.VehicleId.Should().Be(vehicleId);
        response.HighestBid.Should().Be(startingBid);
        response.Status.Should().Be(AuctionStatus.Active);
    }

    [Fact]
    public async Task Handle_ShouldReturnClosedAuctionDetails_WhenAuctionIsClosed()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        decimal startingBid = 10000m;

        var auction = new Auction(vehicleId, startingBid);
        auction.Close();
        var auctionId = auction.Id;

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync(auction);

        var query = new GetAuctionQuery(auctionId);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.AuctionId.Should().Be(auction.Id);
        response.VehicleId.Should().Be(vehicleId);
        response.HighestBid.Should().Be(startingBid);
        response.Status.Should().Be(AuctionStatus.Closed);
    }

    [Fact]
    public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        var query = new GetAuctionQuery(auctionId);

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync((Auction?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuctionNotFoundException>().WithMessage($"*{auctionId}*");
    }

    #endregion
}
