using FluentValidation;

namespace CAMS.Application.Commands.Auctions.PlaceBid
{
    /// <summary>
    /// Validator for the PlaceBidCommand.
    /// </summary>
    public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidCommandValidator()
        {
            RuleFor(cmd => cmd.AuctionId)
                .NotEmpty()
                .WithMessage("AuctionId must not be empty.");

            RuleFor(cmd => cmd.BidAmount)
                .GreaterThan(0)
                .WithMessage("Bid amount must be greater than zero.");

            RuleFor(cmd => cmd.BidderId)
                .NotEmpty()
                .WithMessage("BidderId must not be empty.");
        }
    }
}
