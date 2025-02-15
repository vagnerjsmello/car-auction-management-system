using BCA.CarManagement.Application.Queries.Vehicles.SearchVehicles;
using CAMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CAMS.Application.Queries.Vehicles;

/// <summary>
/// Handler responsible for processing vehicle search queries.
/// </summary>
public class SearchVehiclesQueryHandler
    : IRequestHandler<SearchVehiclesQuery, SearchVehiclesResponse>
{
    private readonly ILogger<SearchVehiclesQueryHandler> _logger;
    private readonly IVehicleRepository _vehicleRepository;

    public SearchVehiclesQueryHandler(ILogger<SearchVehiclesQueryHandler> logger, IVehicleRepository vehicleRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
    }

    public async Task<SearchVehiclesResponse> Handle(SearchVehiclesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SearchVehiclesQuery with filters: {@Query}", query);

        var vehicles = await _vehicleRepository.Search(v =>
            (!query.VehicleType.HasValue || v.Type == query.VehicleType.Value)
            && (string.IsNullOrEmpty(query.Manufacturer)
                 || v.Manufacturer.Equals(query.Manufacturer, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrEmpty(query.Model)
                 || v.Model.Equals(query.Model, StringComparison.OrdinalIgnoreCase))
            && (!query.Year.HasValue || v.Year == query.Year.Value)        );

        _logger.LogInformation("SearchVehiclesQuery returned {Count} vehicles.", vehicles.Count());
        var response = new SearchVehiclesResponse(vehicles);
        return response;
    }
}
