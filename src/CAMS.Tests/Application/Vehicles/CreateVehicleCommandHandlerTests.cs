using CAMS.Application.Commands.Vehicles.CreateVehicle;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Vehicles
{
    /// <summary>
    /// Tests for CreateVehicleCommandHandler ensuring vehicle creation, error scenarios, and validator behavior.
    /// </summary>
    public class CreateVehicleCommandHandlerTests
    {
        private readonly Mock<IVehicleRepository> _repositoryMock;
        private readonly Mock<IValidator<CreateVehicleCommand>> _validatorMock;
        private readonly Mock<ILogger<CreateVehicleCommandHandler>> _loggerMock;
        private readonly CreateVehicleCommandHandler _handler;

        public CreateVehicleCommandHandlerTests()
        {
            _repositoryMock = new Mock<IVehicleRepository>();
            _validatorMock = new Mock<IValidator<CreateVehicleCommand>>();
            _loggerMock = new Mock<ILogger<CreateVehicleCommandHandler>>();

            // Default behavior for the mocked validator: validation is successful.
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<CreateVehicleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _handler = new CreateVehicleCommandHandler(
                _loggerMock.Object,
                _repositoryMock.Object,
                _validatorMock.Object
            );
        }

        #region Handler Constructor Tests

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Arrange
            ILogger<CreateVehicleCommandHandler> nullLogger = null!;

            // Act
            Action act = () => new CreateVehicleCommandHandler(
                nullLogger,
                _repositoryMock.Object,
                _validatorMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Arrange
            IVehicleRepository nullRepository = null!;

            // Act
            Action act = () => new CreateVehicleCommandHandler(
                _loggerMock.Object,
                nullRepository,
                _validatorMock.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("vehicleRepository");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenValidatorIsNull()
        {
            // Arrange
            IValidator<CreateVehicleCommand> nullValidator = null!;

            // Act
            Action act = () => new CreateVehicleCommandHandler(
                _loggerMock.Object,
                _repositoryMock.Object,
                nullValidator);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("validator");
        }

        [Fact]
        public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
        {
            // Act
            Action act = () => new CreateVehicleCommandHandler(
                _loggerMock.Object,
                _repositoryMock.Object,
                _validatorMock.Object);

            // Assert
            act.Should().NotThrow();
        }

        #endregion

        #region Handler Tests

        [Fact]
        public async Task Handle_ShouldCreateVehicleSuccessfully_WhenCommandIsValid()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "Bayon",
                Year = 2022,
                StartingBid = 15000m,
                NumberOfDoors = 4
            };

            var command = new CreateVehicleCommand(request);

            // Simulate that no vehicle exists with this ID.
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vehicle)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("because a valid vehicle should be created");
            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be(command.Id);
            result.Data.VehicleType.Should().Be(VehicleType.Sedan);
            result.Data.Manufacturer.Should().Be("Hyundai");
            result.Data.Model.Should().Be("Bayon");
            result.Data.Year.Should().Be(2022);
            result.Data.NumberOfDoors.Should().Be(4);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once,
                "because the vehicle should be added once");

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce,
                "because the handler should log informational messages during processing"
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenValidationFailsForCreateVehicleCommand()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.Empty, // Invalid ID
                VehicleType = VehicleType.Sedan,
                Manufacturer = "",
                Model = "",
                Year = 1800,
                StartingBid = -100,
                NumberOfDoors = 0
            };
            var command = new CreateVehicleCommand(request);

            var validationFailure = new ValidationFailure("Id", "UniqueId must not be empty.");
            var invalidResult = new ValidationResult(new[] { validationFailure });            
            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(invalidResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("UniqueId must not be empty"));
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldThrowVehicleAlreadyExistsException_WhenVehicleWithSameIdExists()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "Bayon",
                Year = 2022,
                StartingBid = 15000m,
                NumberOfDoors = 4
            };

            var command = new CreateVehicleCommand(request);

            // Simulate an existing vehicle with the same ID.
            var existingVehicle = new Sedan(request.Id, "Test", "Test", 2020, 10000m, 4);
            _repositoryMock.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(existingVehicle);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<VehicleAlreadyExistsException>()
                .WithMessage($"Vehicle with id {command.Id} already exists.");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
        }

        #endregion

        #region Integrated Validator Tests

        // Helper: Real instance of CreateVehicleCommandValidator.
        private CreateVehicleCommandValidator GetRealValidator() => new CreateVehicleCommandValidator();

        [Fact]
        public void Validator_ShouldHaveError_WhenIdIsEmpty()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.Empty,
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "i20",
                Year = 2022,
                StartingBid = 10000m,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenManufacturerIsEmpty()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "",
                Model = "i20",
                Year = 2022,
                StartingBid = 10000m,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.Manufacturer);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenModelIsEmpty()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "",
                Year = 2022,
                StartingBid = 10000m,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.Model);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenYearIsLessThan1900()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "i20",
                Year = 1890,
                StartingBid = 10000m,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.Year);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenStartingBidIsNegative()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "i20",
                Year = 2022,
                StartingBid = -1,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.StartingBid);
        }

        [Fact]
        public void Validator_ShouldHaveError_WhenNumberOfDoorsIsInvalidForSedan()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "i20",
                Year = 2022,
                StartingBid = 10000m,
                NumberOfDoors = 0  // Invalid value
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldHaveValidationErrorFor(c => c.NumberOfDoors);
        }

        [Fact]
        public void Validator_ShouldNotHaveError_WhenAllFieldsAreValidForSedan()
        {
            // Arrange
            var request = new CreateVehicleRequest
            {
                Id = Guid.NewGuid(),
                VehicleType = VehicleType.Sedan,
                Manufacturer = "Hyundai",
                Model = "Bayon",
                Year = 2022,
                StartingBid = 15000m,
                NumberOfDoors = 4
            };
            var command = new CreateVehicleCommand(request);
            var validator = GetRealValidator();

            // Act & Assert
            validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }

        #endregion
    }
}
