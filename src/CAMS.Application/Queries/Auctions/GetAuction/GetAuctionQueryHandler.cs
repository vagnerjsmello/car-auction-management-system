﻿using CAMS.Application.Common;
using CAMS.Domain.Exceptions;
using CAMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CAMS.Application.Queries.Auctions.GetAuction;

/// <summary>
/// Handles the GetAuctionQuery.
/// </summary>
public class GetAuctionQueryHandler : IRequestHandler<GetAuctionQuery, OperationResult<GetAuctionResponse>>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly ILogger<GetAuctionQueryHandler> _logger;

    public GetAuctionQueryHandler(IAuctionRepository auctionRepository, ILogger<GetAuctionQueryHandler> logger)
    {
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OperationResult<GetAuctionResponse>> Handle(GetAuctionQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fetching auction details for AuctionId: {query.AuctionId}");

        var auction = await _auctionRepository.GetByIdAsync(query.AuctionId);
        if (auction == null)
        {            
            var ex = new AuctionNotFoundException(query.AuctionId);
            _logger.LogWarning(ex.Message);
            throw ex;

        }

        var response = new GetAuctionResponse(auction.Id, auction.VehicleId, auction.HighestBid, auction.Status);
        return OperationResult<GetAuctionResponse>.Success(response);
    }
}
