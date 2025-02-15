using CAMS.Domain.Entities;

namespace BCA.CarManagement.Application.Commands.Vehicles.CreateVehicles;

/// <summary>
/// Represents the response returned after a vehicle is created.
/// </summary>
public class CreateVehicleResponse
{
    public Guid Id { get; }
    public VehicleType VehicleType { get; }
    public string Manufacturer { get; } = null!;
    public string Model { get; } = null!;
    public int Year { get; }
    public decimal StartingBid { get; }
    public int NumberOfDoors { get; }
    public int NumberOfSeats { get; }
    public double LoadCapacity { get; }


    public CreateVehicleResponse(Vehicle vehicle)
    {
        
        Id = vehicle.Id;        
        VehicleType = vehicle.Type;
        Manufacturer = vehicle.Manufacturer;
        Model = vehicle.Model;
        Year = vehicle.Year;
        StartingBid = vehicle.StartingBid;
        
        // If Sedan or Hatchback, extract its NumberOfDoors;
        NumberOfDoors = vehicle switch
        {
            Sedan sedan => sedan.NumberOfDoors,
            Hatchback hatchback => hatchback.NumberOfDoors,
            _ => default
        };

        // If SUV, extract its NumberOfSeats; 
        NumberOfSeats = vehicle is SUV suv ? suv.NumberOfSeats : default;
        
        // If Truck, extract its LoadCapacity; 
        LoadCapacity = vehicle is Truck truck ? truck.LoadCapacity : default;

    }
}

