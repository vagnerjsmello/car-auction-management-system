using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

/// <summary>
/// Represents a Sedan vehicle with a specified load capacity.
/// </summary>
public class Truck : Vehicle
{
    public double LoadCapacity { get; }
    public override VehicleType Type => VehicleType.Truck;


    public Truck(Guid id, string manufacturer, string model, int year, decimal startingBid, double loadCapacity)
        : base(id, manufacturer, model, year, startingBid)
    {
        if (loadCapacity <= 0)
            throw new ArgumentException("LoadCapacity must be greater than zero.", nameof(loadCapacity));

        LoadCapacity = loadCapacity;
    }
}
