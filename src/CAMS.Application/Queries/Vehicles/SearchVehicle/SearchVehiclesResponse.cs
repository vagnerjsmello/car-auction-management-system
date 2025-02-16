using CAMS.Domain.Entities;

namespace CAMS.Application.Queries.Vehicles.SearchVehicle;

/// <summary>
/// Represents the response containing the list of vehicles returned from a search operation.
/// </summary>
public class SearchVehiclesResponse
{
    public IEnumerable<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public SearchVehiclesResponse(IEnumerable<Vehicle> vehicles)
    {
        Vehicles = vehicles;
    }
}
