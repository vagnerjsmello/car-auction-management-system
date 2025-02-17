namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when a vehicle with the specified identifier already exists.
/// </summary>
public class VehicleAlreadyExistsException : Exception
{
    private const string DefaultMessage = "Vehicle with the specified id already exists.";

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class
    /// with the default message.
    /// </summary>
    public VehicleAlreadyExistsException() : base(DefaultMessage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class
    /// with a specific vehicle ID, embedding it into the message.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle that already exists.</param>
    public VehicleAlreadyExistsException(Guid vehicleId)
        : base($"Vehicle with id {vehicleId} already exists.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public VehicleAlreadyExistsException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleAlreadyExistsException"/> class
    /// with a custom message and an inner exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public VehicleAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
