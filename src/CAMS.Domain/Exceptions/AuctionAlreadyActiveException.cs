namespace CAMS.Domain.Exceptions;


/// <summary>
/// Exception thrown when an auction is already active for a vehicle.
/// </summary>
public class AuctionAlreadyActiveException : Exception
{
    private const string DefaultMessage = "An active auction already exists for this vehicle.";

    public AuctionAlreadyActiveException() : base(DefaultMessage) { }
    public AuctionAlreadyActiveException(string message) : base(message) { }
    public AuctionAlreadyActiveException(string message, Exception innerException) : base(message, innerException) { }
}