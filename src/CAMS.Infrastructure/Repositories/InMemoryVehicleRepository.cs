using CAMS.Domain.Entities;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;

namespace CAMS.Infrastructure.Repositories
{
    public class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();

        public async Task AddAsync(Vehicle vehicle)
        {
            if (_vehicles.Any(v => v.Id == vehicle.Id))
            {
                throw new VehicleAlreadyExistsException($"A vehicle with id {vehicle.Id} already exists.");
            }
            _vehicles.Add(vehicle);
            await Task.CompletedTask;
        }

        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
            {
                throw new VehicleNotFoundException($"Vehicle with id {id} not found.");
            }
            return await Task.FromResult(vehicle);
        }

        public async Task<IEnumerable<Vehicle>> Search(Func<Vehicle, bool> predicate)
        {
            return await Task.FromResult(_vehicles.Where(predicate));
        }
    }
}
