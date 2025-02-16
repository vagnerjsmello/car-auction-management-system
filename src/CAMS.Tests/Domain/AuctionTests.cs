using System;
using System.Linq;
using CAMS.Domain.Entities;
using CAMS.Domain.Enums;
using CAMS.Domain.Events;
using CAMS.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CAMS.Tests.Domain
{
    /// <summary>
    /// Unit tests for the Auction domain entity.
    /// </summary>
    public class AuctionTests
    {
        [Fact]
        public void Auction_Constructor_ShouldInitializeAuctionAndRaiseAuctionStartedEvent()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;

            // Act
            var auction = new Auction(vehicleId, startingBid);

            // Assert
            auction.VehicleId.Should().Be(vehicleId);
            auction.HighestBid.Should().Be(startingBid);
            auction.Status.Should().Be(AuctionStatus.Active);
            auction.DomainEvents.Should().HaveCount(1);
            auction.DomainEvents.First().Should().BeOfType<AuctionStartedEvent>();

            var startedEvent = auction.DomainEvents.First() as AuctionStartedEvent;
            startedEvent.AuctionId.Should().Be(auction.Id);
            startedEvent.VehicleId.Should().Be(vehicleId);
            startedEvent.StartingBid.Should().Be(startingBid);
        }

        [Fact]
        public void PlaceBid_WithValidBid_ShouldUpdateHighestBidAndRaiseBidPlacedEvent()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;
            var auction = new Auction(vehicleId, startingBid);
            auction.DomainEvents.Clear(); // Clear the initial AuctionStartedEvent for clarity

            var bidAmount = startingBid + 1000;
            var bidderId = Guid.NewGuid();
            var bid = new Bid(bidAmount, bidderId);

            // Act
            auction.PlaceBid(bid);

            // Assert
            auction.HighestBid.Should().Be(bidAmount);
            auction.Bids.Should().Contain(bid);
            auction.DomainEvents.Should().HaveCount(1);
            auction.DomainEvents.First().Should().BeOfType<BidPlacedEvent>();

            var bidPlacedEvent = auction.DomainEvents.First() as BidPlacedEvent;
            bidPlacedEvent.AuctionId.Should().Be(auction.Id);
            bidPlacedEvent.VehicleId.Should().Be(vehicleId);
            bidPlacedEvent.BidAmount.Should().Be(bidAmount);
            bidPlacedEvent.BidderId.Should().Be(bidderId);
            bidPlacedEvent.Timestamp.Should().BeCloseTo(bid.Timestamp, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void PlaceBid_WithInvalidBid_ShouldThrowInvalidBidException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;
            var auction = new Auction(vehicleId, startingBid);
            var bidAmount = startingBid; // Not greater than highest
            var bidderId = Guid.NewGuid();
            var bid = new Bid(bidAmount, bidderId);

            // Act
            Action act = () => auction.PlaceBid(bid);

            // Assert
            act.Should().Throw<InvalidBidException>()
                .WithMessage("*higher than the current highest bid*");
        }

        [Fact]
        public void Close_WhenAuctionIsActive_ShouldSetStatusToClosedAndRaiseAuctionClosedEvent()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;
            var auction = new Auction(vehicleId, startingBid);
            auction.DomainEvents.Clear(); // Clear the initial event

            // Act
            auction.Close();

            // Assert
            auction.Status.Should().Be(AuctionStatus.Closed);
            auction.DomainEvents.Should().HaveCount(1);
            auction.DomainEvents.First().Should().BeOfType<AuctionClosedEvent>();

            var closedEvent = auction.DomainEvents.First() as AuctionClosedEvent;
            closedEvent.AuctionId.Should().Be(auction.Id);
            closedEvent.VehicleId.Should().Be(vehicleId);
            closedEvent.FinalHighestBid.Should().Be(auction.HighestBid);
        }

        [Fact]
        public void Close_WhenAuctionIsAlreadyClosed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;
            var auction = new Auction(vehicleId, startingBid);
            auction.Close(); // Close it once

            // Act
            Action act = () => auction.Close();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*already closed*");
        }

        [Fact]
        public void PlaceBid_ShouldRecordAllBidsAndMaintainOrder()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            decimal startingBid = 10000m;
            var auction = new Auction(vehicleId, startingBid);
            auction.DomainEvents.Clear();

            var bids = new[]
            {
                new Bid(startingBid + 1000, Guid.NewGuid()),
                new Bid(startingBid + 2000, Guid.NewGuid()),
                new Bid(startingBid + 3000, Guid.NewGuid())
            };

            // Act: Place bids sequentially.
            foreach (var bid in bids)
            {
                auction.PlaceBid(bid);
            }

            // Assert: 
            auction.Bids.Should().HaveCount(bids.Length);
            auction.Bids.Select(b => b.Amount).Should().Equal(bids.Select(b => b.Amount));
            auction.HighestBid.Should().Be(bids.Last().Amount);
        }
    }
}
