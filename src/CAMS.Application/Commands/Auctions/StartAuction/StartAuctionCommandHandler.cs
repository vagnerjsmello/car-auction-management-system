using System;
using System.Threading;
using System.Threading.Tasks;
using CAMS.Domain.Entities;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Events;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CAMS.Application.Commands.Auctions.StartAuction;

/// <summary>
/// Handles the StartAuctionCommand logic.
/// </summary>
public class StartAuctionCommandHandler : IRequestHandler<StartAuctionCommand, StartAuctionResponse>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAuctionRepository _auctionRepository;
    private readonly IValidator<StartAuctionCommand> _validator;
    private readonly ILogger<StartAuctionCommandHandler> _logger;
    private readonly IDomainEventPublisher _eventPublisher;

    public StartAuctionCommandHandler(
        IVehicleRepository vehicleRepository, 
        IAuctionRepository auctionRepository, 
        IValidator<StartAuctionCommand> validator, 
        ILogger<StartAuctionCommandHandler> logger, 
        IDomainEventPublisher eventPublisher)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    public async Task<StartAuctionResponse> Handle(StartAuctionCommand command, CancellationToken cancellationToken)
    {
        // Validate the command
        ValidationResult validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for StartAuctionCommand: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Starting auction for vehicle ID: {VehicleId}", command.VehicleId);

        var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId);
        if (vehicle == null)
        {
            _logger.LogWarning("Vehicle with ID {VehicleId} not found.", command.VehicleId);
            throw new VehicleNotFoundException($"Vehicle with id {command.VehicleId} not found.");
        }

        var existingAuction = await _auctionRepository.GetActiveAuctionByVehicleIdAsync(command.VehicleId);
        if (existingAuction != null)
        {
            _logger.LogWarning("Auction is already active for vehicle {VehicleId}.", command.VehicleId);
            throw new AuctionAlreadyActiveException($"An active auction already exists for vehicle {command.VehicleId}.");
        }
        
        var auction = new Auction(command.VehicleId, command.StartingBid);

        await _auctionRepository.AddAsync(auction);
        await _eventPublisher.PublishEventsAsync(auction);

        _logger.LogInformation("Auction {AuctionId} started for vehicle {VehicleId}.", auction.Id, command.VehicleId);

        return new StartAuctionResponse(auction.Id, command.VehicleId, command.StartingBid);
    }
}
