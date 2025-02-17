namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when an auction is not found.
/// </summary>
public class AuctionNotFoundException : Exception
{
    private const string DefaultMessage = "Auction not found.";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionNotFoundException"/> class
    /// with the default message.
    /// </summary>
    public AuctionNotFoundException() : base(DefaultMessage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionNotFoundException"/> class
    /// with a specific auction ID, embedding it into the message.
    /// </summary>
    /// <param name="auctionId">The ID of the auction that was not found.</param>
    public AuctionNotFoundException(Guid auctionId)
        : base($"Auction with id {auctionId} was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionNotFoundException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public AuctionNotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionNotFoundException"/> class
    /// with a custom message and an inner exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public AuctionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
