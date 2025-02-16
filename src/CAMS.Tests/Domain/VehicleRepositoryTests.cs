using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Repositories;
using FluentAssertions;

namespace CAMS.Tests.Domain
{
    /// <summary>
    /// Unit tests for the in-memory implementation of IVehicleRepository.
    /// </summary>
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
            var vehicle = new Hatchback(Guid.NewGuid(), "Hyundai", "Bayon", 2020, 15000m, 4);

            // Act
            await _repository.AddAsync(vehicle);
            var result = await _repository.GetByIdAsync(vehicle.Id);

            // Assert
            result.Should().NotBeNull();
            result.Manufacturer.Should().Be("Hyundai");
            result.Model.Should().Be("Bayon");
            result.Year.Should().Be(2020);
            result.StartingBid.Should().Be(15000m);
        }

        [Fact]
        public async Task AddVehicle_ShouldThrowExceptionForDuplicateId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var sedan1 = new Sedan(id, "Honda", "Accord", 2021, 20000m, 4);
            var sedan2 = new Sedan(id, "Hyundai", "i20", 2022, 25000m, 4);

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
            var sedan = new Sedan(Guid.NewGuid(), "Hyundai", "i20", 2021, 15000m, 4);
            var suv = new SUV(Guid.NewGuid(), "Honda", "CRV", 2020, 20000m, 5);

            await _repository.AddAsync(hatchback);
            await _repository.AddAsync(sedan);
            await _repository.AddAsync(suv);

            // Act
            var results = await _repository.Search(v =>
                v.Manufacturer.Equals("Hyundai", StringComparison.OrdinalIgnoreCase));

            // Assert
            results.Should().ContainSingle();
            var vehicleFound = results.First();
            vehicleFound.Model.Should().Be("i20");
            vehicleFound.Manufacturer.Should().Be("Hyundai");
        }

        [Fact]
        public async Task SearchVehicles_ShouldReturnAllVehiclesWhenNoCriteriaProvided()
        {
            // Arrange
            var hatchback = new Hatchback(Guid.NewGuid(), "Ford", "Fiesta", 2020, 10000m, 4);
            var sedan = new Sedan(Guid.NewGuid(), "Hyundai", "i20", 2021, 15000m, 4);
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
            var sedani20 = new Sedan(Guid.NewGuid(), "Hyundai", "i20", 2021, 15000m, 4);
            var sedanBayon = new Sedan(Guid.NewGuid(), "Hyundai", "Bayon", 2021, 14000m, 4);
            var hatchbackTucson = new Hatchback(Guid.NewGuid(), "Hyundai", "Tucson", 2021, 12000m, 4);
            var suv = new SUV(Guid.NewGuid(), "Honda", "CRV", 2020, 20000m, 5);
            var truck = new Truck(Guid.NewGuid(), "Ford", "F-150", 2021, 25000m, 10000);

            await _repository.AddAsync(sedani20);
            await _repository.AddAsync(sedanBayon);
            await _repository.AddAsync(hatchbackTucson);
            await _repository.AddAsync(suv);
            await _repository.AddAsync(truck);

            // Act
            var results = await _repository.Search(v =>
                v.Manufacturer.Equals("Hyundai", StringComparison.OrdinalIgnoreCase)
                && v.Year == 2021
                && v.Type == VehicleType.Sedan);

            // Assert
            results.Should().HaveCount(2);
            results.Should()
                .OnlyContain(v => v.Type == VehicleType.Sedan &&
                    v.Manufacturer.Equals("Hyundai", StringComparison.OrdinalIgnoreCase) &&
                    v.Year == 2021);
        }

    }
}
