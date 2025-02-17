namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when a bid is invalid.
/// </summary>
public class InvalidBidException : Exception
{
    private const string DefaultMessage = "Invalid bid.";

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidBidException"/> class
    /// with the default message.
    /// </summary>
    public InvalidBidException() : base(DefaultMessage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidBidException"/> class,
    /// embedding the provided <paramref name="reason"/> into the message.
    /// </summary>
    /// <param name="reason">A short description of why the bid is invalid.</param>
    public InvalidBidException(string reason) : base($"Invalid bid: {reason}") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidBidException"/> class,
    /// embedding the specified <paramref name="bidAmount"/> into the message.
    /// </summary>
    /// <param name="bidAmount">The bid amount considered invalid.</param>
    public InvalidBidException(decimal bidAmount) : base($"Invalid bid amount: {bidAmount}") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidBidException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public InvalidBidException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates an <see cref="InvalidBidException"/> indicating the auction is not active.
    /// </summary>
    /// <returns>A new <see cref="InvalidBidException"/> with a standard message.</returns>
    public static InvalidBidException AuctionNotActive() => new InvalidBidException("The auction is not active.");

    /// <summary>
    /// Creates an <see cref="InvalidBidException"/> indicating the bid must exceed
    /// the current highest bid.
    /// </summary>
    /// <returns>A new <see cref="InvalidBidException"/> with a standard message.</returns>
    public static InvalidBidException MustExceedCurrentBid() => new InvalidBidException("Bid must be higher than the current highest bid.");
}
