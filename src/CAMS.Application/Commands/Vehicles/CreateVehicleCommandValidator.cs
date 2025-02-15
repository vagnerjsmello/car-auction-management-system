using CAMS.Domain.Entities;
using FluentValidation;

namespace BCA.CarManagement.Application.Commands.Vehicles.CreateVehicles;

/// <summary>
/// Validator for the CreateVehicleCommand ensuring that all required fields are valid.
/// </summary>
public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotEmpty()
            .WithMessage("UniqueId must not be empty.");

        RuleFor(cmd => cmd.Manufacturer)
            .NotEmpty()
            .WithMessage("Manufacturer must not be empty.");

        RuleFor(cmd => cmd.Model)
            .NotEmpty()
            .WithMessage("Model must not be empty.");

        RuleFor(cmd => cmd.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900.");

        RuleFor(cmd => cmd.StartingBid)
            .GreaterThanOrEqualTo(0)
            .WithMessage("StartingBid must be non-negative.");

        RuleFor(cmd => cmd.NumberOfDoors)
            .GreaterThan(0)
            .When(cmd => cmd.VehicleType == VehicleType.Sedan || cmd.VehicleType == VehicleType.Hatchback)
            .WithMessage("NumberOfDoors must be greater than 0 for Sedan/Hatchback.");
    }
}
