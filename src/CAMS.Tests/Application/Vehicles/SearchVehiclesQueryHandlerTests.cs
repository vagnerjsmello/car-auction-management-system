using CAMS.Application.Queries.Vehicles.SearchVehicle;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CAMS.Tests.Application.Vehicles;

/// <summary>
/// Unit tests for the SearchVehiclesQueryHandler.
/// </summary>
public class SearchVehiclesQueryHandlerTests
{
    private readonly Mock<ILogger<SearchVehiclesQueryHandler>> _loggerMock;
    private readonly Mock<IVehicleRepository> _repositoryMock;
    private readonly SearchVehiclesQueryHandler _handler;
    private readonly List<Vehicle> _vehicles;

    public SearchVehiclesQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<SearchVehiclesQueryHandler>>();
        _repositoryMock = new Mock<IVehicleRepository>();

        // List of vehicles to simulate the repository data.
        _vehicles = new List<Vehicle>
        {
            new Sedan(Guid.NewGuid(), "Hyundai", "Bayon", 2022, 15000m, 4),
            new Hatchback(Guid.NewGuid(), "Hyundai", "i20", 2021, 12000m, 4),
            new SUV(Guid.NewGuid(), "Honda", "CRV", 2022, 20000m, 5),
            new Truck(Guid.NewGuid(), "Ford", "F-150", 2020, 25000m, 10000)
        };

        _repositoryMock.Setup(r => r.Search(It.IsAny<Func<Vehicle, bool>>())).ReturnsAsync((Func<Vehicle, bool> predicate) => _vehicles.Where(predicate));

        _handler = new SearchVehiclesQueryHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    #region constructor tests
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        ILogger<SearchVehiclesQueryHandler> nullLogger = null!;

        // Act
        Action act = () => new SearchVehiclesQueryHandler(nullLogger, _repositoryMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
    {
        // Arrange
        IVehicleRepository nullRepository = null!;

        // Act
        Action act = () => new SearchVehiclesQueryHandler(_loggerMock.Object, nullRepository);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("vehicleRepository");
    }

    [Fact]
    public void Constructor_ShouldNotThrow_WhenAllDependenciesAreProvided()
    {
        // Act
        Action act = () => new SearchVehiclesQueryHandler(_loggerMock.Object, _repositoryMock.Object);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Handler Logic Tests
    [Fact]
    public async Task Handle_ShouldReturnVehiclesFilteredByManufacturer()
    {
        // Arrange
        var queryRequest = new SearchVehiclesRequest
        {
            Manufacturer = "Hyundai"
        };
        var query = new SearchVehiclesQuery(queryRequest);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Vehicles.Should().NotBeEmpty().And.OnlyContain(v =>
            v.Manufacturer.Equals("Hyundai", StringComparison.OrdinalIgnoreCase));
        response.Vehicles.Count().Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnVehiclesFilteredByYearAndType()
    {
        // Arrange
        var queryRequest = new SearchVehiclesRequest
        {
            Year = 2022,
            VehicleType = VehicleType.SUV
        };
        var query = new SearchVehiclesQuery(queryRequest);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert: 
        response.Should().NotBeNull();
        response.Vehicles.Should().ContainSingle()
            .Which.Should().BeOfType<SUV>()
            .And.Subject.As<SUV>().Year.Should().Be(2022);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllVehicles_WhenNoCriteriaProvided()
    {
        // Arrange
        var queryRequest = new SearchVehiclesRequest();
        var query = new SearchVehiclesQuery(queryRequest);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Vehicles.Should().HaveCount(_vehicles.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoVehicleMatchesFilter()
    {
        // Arrange
        var queryRequest = new SearchVehiclesRequest { Manufacturer = "NonExistentManufacturer" };
        var query = new SearchVehiclesQuery(queryRequest);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Vehicles.Should().BeEmpty();
    }

    #endregion
}
