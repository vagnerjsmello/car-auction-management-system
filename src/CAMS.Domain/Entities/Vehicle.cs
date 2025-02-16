using CAMS.Domain.Enums;

namespace CAMS.Domain.Entities;

/// <summary>
/// Represents a vehicle with common attributes.
/// </summary>
public abstract class Vehicle : Entity
{
    public string Manufacturer { get; }
    public string Model { get; }
    public int Year { get; }
    public decimal StartingBid { get; }
    public abstract VehicleType Type { get; }


    protected Vehicle(Guid id, string manufacturer, string model, int year, decimal startingBid)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new ArgumentException("Manufacturer cannot be empty.", nameof(manufacturer));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty.", nameof(model));

        if (year < 1900)
            throw new ArgumentException("Year must be greater than or equal to 1900.", nameof(year));

        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }
}
