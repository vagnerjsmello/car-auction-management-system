using CAMS.Application.Commands.Auctions.StartAuction;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using CAMS.Infrastructure.Events;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CAMS.Application.Commands.Auctions.CloseAuction;

/// <summary>
/// Handles the CloseAuctionCommand logic.
/// </summary>
public class CloseAuctionCommandHandler : IRequestHandler<CloseAuctionCommand, CloseAuctionResponse>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IValidator<CloseAuctionCommand> _validator;
    private readonly ILogger<CloseAuctionCommandHandler> _logger;
    private readonly IDomainEventPublisher _eventPublisher;

    public CloseAuctionCommandHandler(
        IAuctionRepository auctionRepository, 
        IValidator<CloseAuctionCommand> validator, 
        ILogger<CloseAuctionCommandHandler> logger, 
        IDomainEventPublisher eventPublisher)
    {
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    public async Task<CloseAuctionResponse> Handle(CloseAuctionCommand command, CancellationToken cancellationToken)
    {
        
        ValidationResult validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning($"Validation failed for {command.GetType().Name}: {validationResult.Errors}");
            throw new ValidationFailedException(validationResult.Errors);
        }

        _logger.LogInformation($"Closing auction with ID: {command.AuctionId}");

        
        var auction = await _auctionRepository.GetByIdAsync(command.AuctionId);
        if (auction == null)
        {
            var ex = new AuctionNotFoundException(command.AuctionId);
            _logger.LogWarning(ex.Message);
            throw ex;
        }
        
        auction.Close();
        
        await _auctionRepository.UpdateAsync(auction);

        await _eventPublisher.PublishEventsAsync(auction);

        _logger.LogInformation($"Auction {auction.Id} closed successfully.");

        return new CloseAuctionResponse(auction.Id);
    }
}
