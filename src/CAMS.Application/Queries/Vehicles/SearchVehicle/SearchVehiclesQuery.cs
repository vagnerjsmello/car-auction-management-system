using CAMS.Domain.Enums;
using MediatR;

namespace CAMS.Application.Queries.Vehicles.SearchVehicle;

/// <summary>
/// Query object for searching vehicles using optional filters.
/// </summary>
public class SearchVehiclesQuery : IRequest<SearchVehiclesResponse>
{
    public VehicleType? VehicleType { get; }
    public string? Manufacturer { get; }
    public string? Model { get; }
    public int? Year { get; }

    public SearchVehiclesQuery(SearchVehiclesRequest request)
    {
        VehicleType = request.VehicleType;
        Manufacturer = request.Manufacturer;
        Model = request.Model;
        Year = request.Year;
    }
}
