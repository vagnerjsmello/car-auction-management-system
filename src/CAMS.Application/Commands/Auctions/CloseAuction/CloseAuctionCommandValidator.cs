using FluentValidation;

namespace CAMS.Application.Commands.Auctions.CloseAuction;

/// <summary>
/// Validator for the CloseAuctionCommand.
/// </summary>
public class CloseAuctionCommandValidator : AbstractValidator<CloseAuctionCommand>
{
    public CloseAuctionCommandValidator()
    {
        RuleFor(cmd => cmd.AuctionId)
            .NotEmpty().WithMessage("AuctionId must not be empty.");
    }
}
