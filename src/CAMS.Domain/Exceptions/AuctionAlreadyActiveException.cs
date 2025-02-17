namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when an auction is already active for a vehicle.
/// </summary>
public class AuctionAlreadyActiveException : Exception
{
    private const string DefaultMessage = "An active auction already exists for this vehicle.";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionAlreadyActiveException"/> class
    /// with the default message.
    /// </summary>
    public AuctionAlreadyActiveException() : base(DefaultMessage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionAlreadyActiveException"/> class
    /// with a specific vehicle ID, embedding it into the message.
    /// </summary>
    /// <param name="vehicleId">The ID of the vehicle that already has an active auction.</param>
    public AuctionAlreadyActiveException(Guid vehicleId)
        : base($"An active auction already exists for vehicle {vehicleId}.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionAlreadyActiveException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public AuctionAlreadyActiveException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionAlreadyActiveException"/> class
    /// with a custom message and an inner exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public AuctionAlreadyActiveException(string message, Exception innerException) : base(message, innerException) { }
}

