namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when a bid is invalid.
/// </summary>
public class InvalidBidException : Exception
{
    public InvalidBidException(string message) : base(message) { }
    public InvalidBidException(string message, Exception innerException) : base(message, innerException) { }
}