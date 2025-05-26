using GO_Bidding.Services;
using Microsoft.AspNetCore.Mvc;
using GOCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GO_Bidding.Controllers;

[ApiController]
[Route("bidding")]
[Authorize]
public class BiddingController : ControllerBase
{
    private readonly ILogger<BiddingController> _logger;
    private readonly IBiddingRepo _biddingRepo;
    private readonly IBiddingNotification _biddingNotification;

    public BiddingController(
        ILogger<BiddingController> logger,
        IBiddingRepo biddingRepo,
        IBiddingNotification biddingNotification)
    {
        _logger = logger;
        _biddingRepo = biddingRepo;
        _biddingNotification = biddingNotification;
    }

    [Route("bid")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PlaceBid([FromBody] Bidding bid)
    {
        if (bid == null)
        {
            _logger.LogError("Received null bid object in PlaceBid endpoint.");
            return BadRequest("Bid cannot be null");
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != bid.UserId.ToString() && !User.IsInRole("Admin"))
        {
            _logger.LogWarning("User {User} attempted to place a bid on behalf of another user: {BidUserId}", currentUserId, bid.UserId);
            return Forbid();
        }

        var highestBid = _biddingRepo.GetHighestBidByAuctionId(bid.AuctionId);
        if (highestBid != null && bid.Amount <= highestBid.Amount)
        {
            _logger.LogError("Bid rejected: Amount {BidAmount} not greater than current highest bid {HighestBidAmount}. UserId: {UserId}, AuctionId: {AuctionId}, Date: {Date}",
                bid.Amount, highestBid.Amount, bid.UserId, bid.AuctionId, bid.Date);
            return BadRequest("Bid amount must be higher than the current highest bid");
        }

        await _biddingRepo.PlaceBid(bid);
        _logger.LogInformation("New bid placed. BidId: {BidId}, Amount: {Amount}, AuctionId: {AuctionId}, UserId: {UserId}, Date: {Date}",
            bid.Id, bid.Amount, bid.AuctionId, bid.UserId, bid.Date);

        await _biddingNotification.SendBidding(bid);
        return Ok(bid);
    }

    [Route("bid")]
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteBid([FromBody] Bidding bid)
    {
        if (bid == null)
        {
            _logger.LogError("Attempted to delete a null bid object.");
            return BadRequest("Bid cannot be null");
        }

        _biddingRepo.DeleteBid(bid);
        _logger.LogInformation("Bid deleted. BidId: {BidId}, AuctionId: {AuctionId}, UserId: {UserId}", bid.Id, bid.AuctionId, bid.UserId);

        return Ok(bid);
    }

    [Route("bids/auction/{auctionId}")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllBidsByAuctionId(Guid auctionId)
    {
        if (auctionId == Guid.Empty)
        {
            _logger.LogError("GetAllBidsByAuctionId called with empty AuctionId.");
            return BadRequest("Auction ID cannot be empty");
        }

        var bids = _biddingRepo.GetAllBidsByAuctionId(auctionId);
        _logger.LogInformation("Fetched {Count} bids for AuctionId: {AuctionId}", bids?.Count() ?? 0, auctionId);

        return Ok(bids);
    }

    [Route("bids/user/{userId}")]
    [HttpGet]
    [Authorize]
    public IActionResult GetAllBidsByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            _logger.LogError("GetAllBidsByUserId called with empty UserId.");
            return BadRequest("User ID cannot be empty");
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && currentUserId != userId.ToString())
        {
            _logger.LogWarning("User {CurrentUserId} attempted to access bids for UserId {UserId} without permission.", currentUserId, userId);
            return Forbid();
        }

        var bids = _biddingRepo.GetAllBidsByUserId(userId);
        _logger.LogInformation("Fetched {Count} bids for UserId: {UserId}", bids?.Count() ?? 0, userId);

        return Ok(bids);
    }

    [Route("bid/highest/{auctionId}")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetHighestBidByAuctionId(Guid auctionId)
    {
        if (auctionId == Guid.Empty)
        {
            _logger.LogError("GetHighestBidByAuctionId called with empty AuctionId.");
            return BadRequest("Auction ID cannot be empty");
        }

        var highestBid = _biddingRepo.GetHighestBidByAuctionId(auctionId);

        if (highestBid == null)
        {
            _logger.LogInformation("No bids found for AuctionId: {AuctionId}", auctionId);
            return NotFound(); // <--- vigtigt!
        }

        _logger.LogInformation("Highest bid for AuctionId {AuctionId}: BidId {BidId}, Amount {Amount}, UserId {UserId}",
            auctionId, highestBid.Id, highestBid.Amount, highestBid.UserId);

        return Ok(highestBid);
    }

}