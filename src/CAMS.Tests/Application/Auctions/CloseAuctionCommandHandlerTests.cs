using CAMS.Application.Commands.Auctions.CloseAuction;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Events;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Auctions;

/// <summary>
/// Unit tests for the CloseAuctionCommandHandler.
/// </summary>
public class CloseAuctionCommandHandlerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<IValidator<CloseAuctionCommand>> _validatorMock;
    private readonly Mock<ILogger<CloseAuctionCommandHandler>> _loggerMock;
    private readonly Mock<IDomainEventPublisher> _eventPublisherMock;
    private readonly CloseAuctionCommandHandler _handler;

    public CloseAuctionCommandHandlerTests()
    {
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _validatorMock = new Mock<IValidator<CloseAuctionCommand>>();
        _loggerMock = new Mock<ILogger<CloseAuctionCommandHandler>>();
        _eventPublisherMock = new Mock<IDomainEventPublisher>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CloseAuctionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new CloseAuctionCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Act
        Action act = () => new CloseAuctionCommandHandler(null!, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("auctionRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenValidatorIsNull()
    {
        // Act
        Action act = () => new CloseAuctionCommandHandler(_auctionRepositoryMock.Object, null!, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act
        Action act = () => new CloseAuctionCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, null!, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenEventPublisherIsNull()
    {
        // Act
        Action act = () => new CloseAuctionCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("eventPublisher");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new CloseAuctionCommandHandler(_auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests

    [Fact]
    public async Task Handle_ShouldCloseAuctionSuccessfully_WhenAuctionIsActive()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        // Create an auction with Active status.
        var auction = new Auction(Guid.NewGuid(), 10000m);
        auction.Status.Should().Be(AuctionStatus.Active);

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync(auction);

        var request = new CloseAuctionRequest { AuctionId = auctionId };
        var command = new CloseAuctionCommand(request);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.AuctionId.Should().Be(auction.Id);
        auction.Status.Should().Be(AuctionStatus.Closed);
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Once);
        _eventPublisherMock.Verify(ep => ep.PublishEventsAsync(auction), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        var request = new CloseAuctionRequest { AuctionId = auctionId };
        var command = new CloseAuctionCommand(request);

        _auctionRepositoryMock.Setup(r => r.GetByIdAsync(auctionId)).ReturnsAsync((Auction?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuctionNotFoundException>().WithMessage($"*{auctionId}*");
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Never);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandFailsValidation()
    {
        // Arrange
        var request = new CloseAuctionRequest { AuctionId = Guid.Empty };
        var command = new CloseAuctionCommand(request);

        var validationFailure = new ValidationFailure("AuctionId", "AuctionId must not be empty.");
        var invalidResult = new ValidationResult(new[] { validationFailure });
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(invalidResult);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("*AuctionId must not be empty*");
        _auctionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Auction>()), Times.Never);
    }

    #endregion
}
