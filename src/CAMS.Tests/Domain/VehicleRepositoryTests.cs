using CAMS.Domain.Entities;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Repositories;
using FluentAssertions;

namespace CAMS.Tests.Domain
{
    public class VehicleRepositoryTests
    {
        private readonly IVehicleRepository _repository;

        public VehicleRepositoryTests()
        {
            _repository = new InMemoryVehicleRepository();
        }

        [Fact]
        public async Task AddVehicle_ShouldAddVehicleSuccessfully()
        {
            // Arrange
            var vehicle = new Hatchback(Guid.NewGuid(), "Toyota", "Corolla", 2020, 15000m, 4);

            // Act
            await _repository.AddAsync(vehicle);
            var result = await _repository.GetByIdAsync(vehicle.Id);

            // Assert
            result.Should().NotBeNull();
            result.Manufacturer.Should().Be("Toyota");
            result.Model.Should().Be("Corolla");
            result.Year.Should().Be(2020);
            result.StartingBid.Should().Be(15000m);
        }

        [Fact]
        public async Task AddVehicle_ShouldThrowExceptionForDuplicateId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var sedan1 = new Sedan(id, "Honda", "Accord", 2021, 20000m, 4);
            var sedan2 = new Sedan(id, "Toyota", "Camry", 2022, 25000m, 4);

            await _repository.AddAsync(sedan1);

            // Act
            Func<Task> act = async () => await _repository.AddAsync(sedan2);

            // Assert
            await act.Should().ThrowAsync<VehicleAlreadyExistsException>();
        }

        [Fact]
        public async Task SearchVehicles_ShouldReturnVehiclesByManufacturer()
        {
            // Arrange
            var hatchback = new Hatchback(Guid.NewGuid(), "Ford", "Fiesta", 2020, 10000m, 4);
            var sedan = new Sedan(Guid.NewGuid(), "Toyota", "Camry", 2021, 15000m, 4);
            var suv = new SUV(Guid.NewGuid(), "Honda", "CRV", 2020, 20000m, 5);

            await _repository.AddAsync(hatchback);
            await _repository.AddAsync(sedan);
            await _repository.AddAsync(suv);

            // Act
            var results = await _repository.Search(v =>
                v.Manufacturer.Equals("Toyota", StringComparison.OrdinalIgnoreCase));

            // Assert
            results.Should().ContainSingle();
            var vehicleFound = results.First();
            vehicleFound.Model.Should().Be("Camry");
            vehicleFound.Manufacturer.Should().Be("Toyota");
        }

        [Fact]
        public async Task SearchVehicles_ShouldReturnAllVehiclesWhenNoCriteriaProvided()
        {
            // Arrange
            var hatchback = new Hatchback(Guid.NewGuid(), "Ford", "Fiesta", 2020, 10000m, 4);
            var sedan = new Sedan(Guid.NewGuid(), "Toyota", "Camry", 2021, 15000m, 4);
            var suv = new SUV(Guid.NewGuid(), "Honda", "CRV", 2020, 20000m, 5);

            await _repository.AddAsync(hatchback);
            await _repository.AddAsync(sedan);
            await _repository.AddAsync(suv);

            // Act
            var results = await _repository.Search(v => true);

            // Assert
            results.Count().Should().Be(3);
        }

        [Fact]
        public async Task SearchVehicles_ShouldReturnVehiclesMatchingMultipleCriteria()
        {
            // Arrange
            var sedanCamry = new Sedan(Guid.NewGuid(), "Toyota", "Camry", 2021, 15000m, 4);
            var sedanCorolla = new Sedan(Guid.NewGuid(), "Toyota", "Corolla", 2021, 14000m, 4);
            var hatchbackYaris = new Hatchback(Guid.NewGuid(), "Toyota", "Yaris", 2021, 12000m, 4);
            var suv = new SUV(Guid.NewGuid(), "Honda", "CRV", 2020, 20000m, 5);
            var truck = new Truck(Guid.NewGuid(), "Ford", "F-150", 2021, 25000m, 10000);

            // Add in memory
            await _repository.AddAsync(sedanCamry);
            await _repository.AddAsync(sedanCorolla);
            await _repository.AddAsync(hatchbackYaris);
            await _repository.AddAsync(suv);
            await _repository.AddAsync(truck);

            // Act: Get "Toyota", 2021 year and Sedan type.
            var results = await _repository.Search(v =>
                v.Manufacturer.Equals("Toyota", StringComparison.OrdinalIgnoreCase)
                && v.Year == 2021
                && v.Type == VehicleType.Sedan);

            // Assert
            results.Should().HaveCount(2);
            results.Should()
                .OnlyContain(v => v.Type == VehicleType.Sedan &&
                    v.Manufacturer.Equals("Toyota", StringComparison.OrdinalIgnoreCase) &&
                    v.Year == 2021);
        }

    }
}
