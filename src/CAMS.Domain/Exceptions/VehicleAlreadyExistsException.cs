namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when a vehicle with the specified identifier already exists.
/// </summary>
public class VehicleAlreadyExistsException : Exception
{
    private const string DefaultMessage = "A vehicle with the specified identifier already exists.";
    
    public VehicleAlreadyExistsException() : base(DefaultMessage){ }

    
    public VehicleAlreadyExistsException(string message) : base(message) { }

    public VehicleAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
