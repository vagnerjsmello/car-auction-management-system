using CAMS.Domain.Entities;

namespace BCA.CarManagement.Application.Queries.Vehicles.SearchVehicles;

/// <summary>
/// Query object for searching vehicles using optional filters.
/// </summary>
public class SearchVehiclesRequest
{
    public VehicleType? VehicleType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
}
