using CAMS.Application.Common;
using CAMS.Domain.Enums;
using MediatR;

namespace CAMS.Application.Commands.Vehicles.CreateVehicle;

/// <summary>
/// Command that triggers the creation of a vehicle.
/// </summary>
public class CreateVehicleCommand : IRequest<OperationResult<CreateVehicleResponse>>
{
    public Guid Id { get; }
    public VehicleType VehicleType { get; }
    public string Manufacturer { get; }
    public string Model { get; }
    public int Year { get; }
    public decimal StartingBid { get; }
    public int NumberOfDoors { get; }
    public int NumberOfSeats { get; }
    public double LoadCapacity { get; }


    public CreateVehicleCommand(CreateVehicleRequest request)
    {
        Id = request.Id;
        VehicleType = request.VehicleType;
        Manufacturer = request.Manufacturer;
        Model = request.Model;
        Year = request.Year;
        StartingBid = request.StartingBid;
        NumberOfDoors = request.NumberOfDoors;
        NumberOfSeats = request.NumberOfSeats;
        LoadCapacity = request.LoadCapacity;
    }
}
