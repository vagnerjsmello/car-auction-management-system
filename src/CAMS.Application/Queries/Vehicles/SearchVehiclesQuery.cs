using BCA.CarManagement.Application.Queries.Vehicles.SearchVehicles;
using CAMS.Domain.Entities;
using MediatR;

namespace CAMS.Application.Queries.Vehicles;

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
        this.VehicleType = request.VehicleType;
        this.Manufacturer = request.Manufacturer;
        this.Model = request.Model;
        this.Year = request.Year;
    }
}
