using FluentValidation;

namespace CAMS.Application.Commands.Auctions.StartAuction;


/// <summary>
/// Validator for StartAuctionCommand.
/// </summary>
public class StartAuctionCommandValidator : AbstractValidator<StartAuctionCommand>
{
    public StartAuctionCommandValidator()
    {
        RuleFor(cmd => cmd.VehicleId)
            .NotEmpty()
            .WithMessage("VehicleId must not be empty.");

        RuleFor(cmd => cmd.StartingBid)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Starting bid must be non-negative.");
    }
}
