namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when the requested vehicle is not found.
/// </summary>
public class VehicleNotFoundException : Exception
{
    private const string DefaultMessage = "The requested vehicle was not found.";
   
    public VehicleNotFoundException() : base(DefaultMessage) { }

    public VehicleNotFoundException(string message) : base(message) { }

    public VehicleNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
