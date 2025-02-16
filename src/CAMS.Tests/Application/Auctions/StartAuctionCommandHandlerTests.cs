using CAMS.Application.Commands.Auctions.StartAuction;
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
/// Unit tests for the StartAuctionCommandHandler.
/// </summary>
public class StartAuctionCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly Mock<IValidator<StartAuctionCommand>> _validatorMock;
    private readonly Mock<ILogger<StartAuctionCommandHandler>> _loggerMock;
    private readonly Mock<IDomainEventPublisher> _eventPublisherMock;
    private readonly StartAuctionCommandHandler _handler;

    public StartAuctionCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _validatorMock = new Mock<IValidator<StartAuctionCommand>>();
        _loggerMock = new Mock<ILogger<StartAuctionCommandHandler>>();
        _eventPublisherMock = new Mock<IDomainEventPublisher>();

        // Default: command validation is successful.
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<StartAuctionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new StartAuctionCommandHandler(
            _vehicleRepositoryMock.Object,
            _auctionRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object,
            _eventPublisherMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenVehicleRepositoryIsNull()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(null!, _auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("vehicleRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenAuctionRepositoryIsNull()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(_vehicleRepositoryMock.Object, null!, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("auctionRepository");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenValidatorIsNull()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object, null!, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object, _validatorMock.Object, null!, _eventPublisherMock.Object);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenEventPublisherIsNull()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, null!);
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("eventPublisher");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new StartAuctionCommandHandler(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests

    [Fact]
    public async Task Handle_ShouldCreateAuctionSuccessfully_WhenCommandIsValid()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        decimal startingBid = 10000m;
        var request = new StartAuctionRequest { VehicleId = vehicleId, StartingBid = startingBid };
        var command = new StartAuctionCommand(request);

        var vehicle = new Sedan(vehicleId, "Hyundai", "Bayon", 2022, startingBid, 4);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);

        _auctionRepositoryMock.Setup(r => r.GetActiveAuctionByVehicleIdAsync(vehicleId)).ReturnsAsync((Auction?)null);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.VehicleId.Should().Be(vehicleId);
        response.StartingBid.Should().Be(startingBid);
        response.AuctionId.Should().NotBeEmpty();

        _auctionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Auction>()), Times.Once);
        _eventPublisherMock.Verify(ep => ep.PublishEventsAsync(It.IsAny<Auction>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowVehicleNotFoundException_WhenVehicleDoesNotExist()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var request = new StartAuctionRequest { VehicleId = vehicleId, StartingBid = 10000m };
        var command = new StartAuctionCommand(request);

        // Simulate vehicle not found.
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync((Vehicle?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<VehicleNotFoundException>().WithMessage($"*{vehicleId}*");
        _auctionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Auction>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowAuctionAlreadyActiveException_WhenActiveAuctionExists()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        decimal startingBid = 20000m;
        var request = new StartAuctionRequest { VehicleId = vehicleId, StartingBid = startingBid };
        var command = new StartAuctionCommand(request);

        var vehicle = new Sedan(vehicleId, "Hyundai", "Bayon", 2022, startingBid, 4);
        _vehicleRepositoryMock.Setup(r => r.GetByIdAsync(vehicleId)).ReturnsAsync(vehicle);

        // When existing active auction.
        var existingAuction = new Auction(vehicleId, startingBid);
        _auctionRepositoryMock.Setup(r => r.GetActiveAuctionByVehicleIdAsync(vehicleId)).ReturnsAsync(existingAuction);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuctionAlreadyActiveException>().WithMessage($"*{vehicleId}*");
        _auctionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Auction>()), Times.Never);
    }

    #endregion

    #region Validator Tests

    [Fact]
    public void Validator_ShouldHaveError_WhenVehicleIdIsEmpty()
    {
        // Arrange
        var request = new StartAuctionRequest { VehicleId = Guid.Empty, StartingBid = 10000m };
        var command = new StartAuctionCommand(request);
        var validator = new StartAuctionCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.VehicleId);
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenStartingBidIsNegative()
    {
        // Arrange
        var request = new StartAuctionRequest { VehicleId = Guid.NewGuid(), StartingBid = -1m };
        var command = new StartAuctionCommand(request);
        var validator = new StartAuctionCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.StartingBid);
    }

    [Fact]
    public void Validator_ShouldNotHaveAnyErrors_WhenCommandIsValid()
    {
        // Arrange
        var request = new StartAuctionRequest { VehicleId = Guid.NewGuid(), StartingBid = 10000m };
        var command = new StartAuctionCommand(request);
        var validator = new StartAuctionCommandValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
