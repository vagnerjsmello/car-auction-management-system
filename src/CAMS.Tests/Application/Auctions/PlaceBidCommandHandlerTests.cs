using CAMS.Application.Commands.Auctions.PlaceBid;
using CAMS.Domain.Entities;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Events;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Auctions;

/// <summary>
/// Unit tests for the PlaceBidCommandHandler.
/// </summary>
public class PlaceBidCommandHandlerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<IValidator<PlaceBidCommand>> _validatorMock;
    private readonly Mock<ILogger<PlaceBidCommandHandler>> _loggerMock;
    private readonly Mock<IDomainEventPublisher> _eventPublisherMock;
    private readonly PlaceBidCommandHandler _handler;

    public PlaceBidCommandHandlerTests()
    {
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _validatorMock = new Mock<IValidator<PlaceBidCommand>>();
        _loggerMock = new Mock<ILogger<PlaceBidCommandHandler>>();
        _eventPublisherMock = new Mock<IDomainEventPublisher>();

        // Default: command validation is successful.
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<PlaceBidCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new PlaceBidCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Act
        Action act = () => new PlaceBidCommandHandler(null!, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("auctionRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenValidatorIsNull()
    {
        // Act
        Action act = () => new PlaceBidCommandHandler(_auctionRepositoryMock.Object, null!, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act
        Action act = () => new PlaceBidCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, null!, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenEventPublisherIsNull()
    {
        // Act
        Action act = () => new PlaceBidCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("eventPublisher");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new PlaceBidCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests

    [Fact]
    public async Task Handle_ShouldPlaceBidSuccessfully_WhenBidIsValid()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        decimal startingBid = 10000m;
        decimal bidAmount = startingBid + 1000;
        var bidderId = Guid.NewGuid();

        var auction = new Auction(Guid.NewGuid(), startingBid);
        auction.DomainEvents.Clear();

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync(auction);

        // Create the command.
        var request = new PlaceBidRequest { AuctionId = auctionId, BidAmount = bidAmount, BidderId = bidderId };
        var command = new PlaceBidCommand(request);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();        
        response.Data.AuctionId.Should().Be(auction.Id);
        response.Data.HighestBid.Should().Be(bidAmount);
        auction.Bids.Should().ContainSingle(b => b.Amount == bidAmount && b.BidderId == bidderId);
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Once);
        _eventPublisherMock.Verify(ep => ep.PublishEventsAsync(auction), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        var request = new PlaceBidRequest { AuctionId = auctionId, BidAmount = 11000m, BidderId = Guid.NewGuid() };
        var command = new PlaceBidCommand(request);

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync((Auction?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuctionNotFoundException>().WithMessage($"*{auctionId}*");
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidBidException_WhenBidIsNotHigherThanCurrent()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        decimal startingBid = 10000m;
        var auction = new Auction(Guid.NewGuid(), startingBid);
        auction.DomainEvents.Clear();

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync(auction);

        // Bid that is not higher than the current highest bid.
        var request = new PlaceBidRequest { AuctionId = auctionId, BidAmount = startingBid, BidderId = Guid.NewGuid() };
        var command = new PlaceBidCommand(request);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidBidException>().WithMessage("*higher than the current highest bid*");
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Never);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenValidationFailsForPlaceBidCommand()
    {
        // Arrange
        var request = new PlaceBidRequest { AuctionId = Guid.Empty, BidAmount = -100, BidderId = Guid.Empty };
        var command = new PlaceBidCommand(request);

        var validationFailure = new ValidationFailure("AuctionId", "AuctionId must not be empty.");
        var invalidResult = new ValidationResult(new[] { validationFailure });        
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(invalidResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("AuctionId must not be empty"));
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Never);
    }


    [Fact]
    public void Validator_ShouldHaveError_WhenAuctionIdIsEmpty()
    {
        // Arrange
        var request = new PlaceBidRequest { AuctionId = Guid.Empty, BidAmount = 100m, BidderId = Guid.NewGuid() };
        var command = new PlaceBidCommand(request);
        var validator = new PlaceBidCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.AuctionId);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenBidAmountIsZeroOrLess()
    {
        // Arrange
        var request = new PlaceBidRequest { AuctionId = Guid.NewGuid(), BidAmount = 0m, BidderId = Guid.NewGuid() };
        var command = new PlaceBidCommand(request);
        var validator = new PlaceBidCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.BidAmount);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenBidderIdIsEmpty()
    {
        // Arrange
        var request = new PlaceBidRequest { AuctionId = Guid.NewGuid(), BidAmount = 100m, BidderId = Guid.Empty };
        var command = new PlaceBidCommand(request);
        var validator = new PlaceBidCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.BidderId);
    }

    [Fact]
    public void Validator_ShouldNotHaveAnyErrors_WhenCommandIsValid()
    {
        // Arrange
        var request = new PlaceBidRequest { AuctionId = Guid.NewGuid(), BidAmount = 200m, BidderId = Guid.NewGuid() };
        var command = new PlaceBidCommand(request);
        var validator = new PlaceBidCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
