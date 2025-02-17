namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when the requested vehicle is not found.
/// </summary>
public class VehicleNotFoundException : Exception
{
    private const string DefaultMessage = "The requested vehicle was not found.";

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class
    /// with the default message.
    /// </summary>
    public VehicleNotFoundException() : base(DefaultMessage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class
    /// with a specific vehicle ID, embedding it into the message.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle that was not found.</param>
    public VehicleNotFoundException(Guid vehicleId)
        : base($"Vehicle with id {vehicleId} was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public VehicleNotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleNotFoundException"/> class
    /// with a custom message and an inner exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public VehicleNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
