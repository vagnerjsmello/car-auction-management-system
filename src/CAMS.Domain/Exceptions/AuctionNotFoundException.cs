namespace CAMS.Domain.Exceptions;

/// <summary>
/// Exception thrown when an auction is not found.
/// </summary>
public class AuctionNotFoundException : Exception
    {
        private const string DefaultMessage = "Auction not found.";

        public AuctionNotFoundException() : base(DefaultMessage) { }
        public AuctionNotFoundException(string message) : base(message) { }
        public AuctionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

