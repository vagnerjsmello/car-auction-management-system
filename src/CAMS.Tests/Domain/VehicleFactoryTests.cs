using CAMS.Domain.Entities;
using CAMS.Domain.Factories;
using FluentAssertions;

namespace CAMS.Tests.Domain;

public class VehicleFactoryTests
{
    [Theory]
    [InlineData(VehicleType.Hatchback, 4, 0, 0)]
    [InlineData(VehicleType.Sedan, 4, 0, 0)]
    [InlineData(VehicleType.SUV, 0, 7, 0)]
    [InlineData(VehicleType.Truck, 0, 0, 10000)]
    public void CreateVehicle_WithVariousTypes_ReturnsCorrectInstance(
        VehicleType vehicleType,
        int numberOfDoors,
        int numberOfSeats,
        double loadCapacity)
    {
        // Arrange
        var uniqueId = Guid.NewGuid();
        var manufacturer = "TestManufacturer";
        var model = "TestModel";
        int year = 2022;
        decimal startingBid = 30000m;

        // Act
        var vehicle = VehicleFactory.CreateVehicle(
            vehicleType,
            uniqueId,
            manufacturer,
            model,
            year,
            startingBid,
            numberOfDoors,
            numberOfSeats,
            loadCapacity);

        // Assert: atributos comuns
        vehicle.Should().NotBeNull();
        vehicle.Id.Should().Be(uniqueId);
        vehicle.Manufacturer.Should().Be(manufacturer);
        vehicle.Model.Should().Be(model);
        vehicle.Year.Should().Be(year);
        vehicle.StartingBid.Should().Be(startingBid);

        // Assert: validação do tipo específico e atributo relacionado
        switch (vehicleType)
        {
            case VehicleType.Hatchback:
                vehicle.Should().BeOfType<Hatchback>();
                var hatchback = vehicle as Hatchback;
                hatchback.NumberOfDoors.Should().Be(numberOfDoors);
                break;
            case VehicleType.Sedan:
                vehicle.Should().BeOfType<Sedan>();
                var sedan = vehicle as Sedan;
                sedan.NumberOfDoors.Should().Be(numberOfDoors);
                break;
            case VehicleType.SUV:
                vehicle.Should().BeOfType<SUV>();
                var suv = vehicle as SUV;
                suv.NumberOfSeats.Should().Be(numberOfSeats);
                break;
            case VehicleType.Truck:
                vehicle.Should().BeOfType<Truck>();
                var truck = vehicle as Truck;
                truck.LoadCapacity.Should().Be(loadCapacity);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [Fact]
    public void CreateVehicle_ShouldThrowException_WhenInvalidVehicleTypeIsProvided()
    {
        // Arrange
        var uniqueId = Guid.NewGuid();
        var manufacturer = "Test Manufacturer";
        var model = "Test Model";
        int year = 2022;
        decimal startingBid = 30000m;

        // Act
        Action act = () => VehicleFactory.CreateVehicle((VehicleType)999, uniqueId, manufacturer, model, year, startingBid, 0, 0, 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    
    [Fact]
    public void Sedan_WithZeroNumberOfDoors_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Honda";
        string model = "Accord";
        int year = 2022;
        decimal startingBid = 20000m;
        int invalidNumberOfDoors = 0; // Invalid value

        // Act
        Action act = () => new Sedan(id, manufacturer, model, year, startingBid, invalidNumberOfDoors);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*NumberOfDoors*");
    }
    
    [Fact]
    public void Hatchback_WithZeroNumberOfDoors_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Toyota";
        string model = "Yaris";
        int year = 2022;
        decimal startingBid = 15000m;
        int invalidNumberOfDoors = 0; // Invalid value

        // Act
        Action act = () => new Hatchback(id, manufacturer, model, year, startingBid, invalidNumberOfDoors);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*NumberOfDoors*");
    }

    
    [Fact]
    public void SUV_WithZeroNumberOfSeats_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Toyota";
        string model = "RAV4";
        int year = 2022;
        decimal startingBid = 30000m;
        int invalidNumberOfSeats = 0; // Invalid value

        // Act
        Action act = () => new SUV(id, manufacturer, model, year, startingBid, invalidNumberOfSeats);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*NumberOfSeats*");
    }
    
    [Fact]
    public void Truck_WithNonPositiveLoadCapacity_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Ford";
        string model = "F-150";
        int year = 2022;
        decimal startingBid = 25000m;
        double invalidLoadCapacity = 0; // Invalid value (should be positive)

        // Act
        Action act = () => new Truck(id, manufacturer, model, year, startingBid, invalidLoadCapacity);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*LoadCapacity*");
    }
    
    [Fact]
    public void Vehicle_WithEmptyManufacturer_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string invalidManufacturer = ""; // Invalid value
        string model = "Civic";
        int year = 2022;
        decimal startingBid = 20000m;
        int numberOfDoors = 4;

        // Act
        Action act = () => new Sedan(id, invalidManufacturer, model, year, startingBid, numberOfDoors);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Manufacturer*");
    }
    
    [Fact]
    public void Vehicle_WithEmptyModel_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Honda";
        string invalidModel = ""; // Invalid value
        int year = 2022;
        decimal startingBid = 20000m;
        int numberOfDoors = 4;

        // Act
        Action act = () => new Sedan(id, manufacturer, invalidModel, year, startingBid, numberOfDoors);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Model*");
    }

    [Fact]
    public void Vehicle_WithYearLessThan1900_ShouldThrowArgumentException()
    {
        // Arrange
        var id = Guid.NewGuid();
        string manufacturer = "Honda";
        string model = "Civic";
        int invalidYear = 1899; // Invalid year value
        decimal startingBid = 20000m;
        int numberOfDoors = 4;

        // Act
        Action act = () => new Sedan(id, manufacturer, model, invalidYear, startingBid, numberOfDoors);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Year*");
    }
}


