using CAMS.Domain.Entities;

namespace BCA.CarManagement.Application.Commands.Vehicles.CreateVehicles;

/// <summary>
/// Request DTO for creating a vehicle.
/// </summary>
public class CreateVehicleRequest
{
    public Guid Id { get; set; }
    public VehicleType VehicleType { get; set; }
    public string Manufacturer { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public decimal StartingBid { get; set; }
    public int NumberOfDoors { get; set; }
    public int NumberOfSeats { get; set; }
    public double LoadCapacity { get; set; }
}
