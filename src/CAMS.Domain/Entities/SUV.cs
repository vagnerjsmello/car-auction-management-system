namespace CAMS.Domain.Entities;

/// <summary>
/// Represents a Sedan vehicle with a specified number of seats.
/// </summary>
public class SUV : Vehicle
{
    public int NumberOfSeats { get; }
    public override VehicleType Type => VehicleType.SUV;

    public SUV(Guid id, string manufacturer, string model, int year, decimal startingBid, int numberOfSeats)
        : base(id, manufacturer, model, year, startingBid)
    {
        if (numberOfSeats <= 0)
            throw new ArgumentException("NumberOfSeats must be greater than zero.", nameof(numberOfSeats));

        NumberOfSeats = numberOfSeats;
    }
}
