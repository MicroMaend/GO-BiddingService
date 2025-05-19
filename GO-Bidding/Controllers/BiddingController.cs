using GO_Bidding.Services;
using Microsoft.AspNetCore.Mvc;
using GOCore;
namespace GO_Bidding.Controllers;

[ApiController]
[Route("bidding")]
public class BiddingController : ControllerBase
{

    private readonly ILogger<BiddingController> _logger;
    private readonly IBiddingRepo _biddingRepo;
    private readonly BiddingNotification _biddingNotification;

    public BiddingController(ILogger<BiddingController> logger, IBiddingRepo biddingRepo,BiddingNotification biddingNotification)
    {
        _biddingRepo = biddingRepo;
        _logger = logger;
        _biddingNotification = biddingNotification;
    }
    

    [Route("PlaceBid")]
    [HttpPost]
    public async Task<IActionResult> PlaceBid([FromBody] Bidding bid)
    {
        if (bid == null)
        {
            _logger.LogError("Bid cannot be null");
            return BadRequest("Bid cannot be null");
        }
        
        var higgestBid=_biddingRepo.GetHighestBidByAuctionId(bid.AuctionId);
        if (higgestBid != null && bid.Amount <= higgestBid.Amount)
        {
            _logger.LogError("Bid amount must be higher than the current highest bid");
            return BadRequest("Bid amount must be higher than the current highest bid");
        }

        await _biddingRepo.PlaceBid(bid);
        await _biddingNotification.SendBidding(bid);
        return Ok(bid);
    }
    
    [Route("DeleteBid")]
    [HttpDelete]
    public IActionResult DeleteBid([FromBody] Bidding bid)
    {
        if (bid == null)
        {
            _logger.LogError("Bid cannot be null");
            return BadRequest("Bid cannot be null");
        }

        _biddingRepo.DeleteBid(bid);
        return Ok(bid);
    }
    
    [Route("GetAllBidsByAuctionId")]
    [HttpGet]
    public IActionResult GetAllBidsByAuctionId([FromQuery] Guid auctionId)
    {
        if (auctionId == Guid.Empty)
        {
            _logger.LogError("Auction ID cannot be empty");
            return BadRequest("Auction ID cannot be empty");
        }

        var bids = _biddingRepo.GetAllBidsByAuctionId(auctionId);
        return Ok(bids);
    }
    
    [Route("GetAllBidsByUserId")]
    [HttpGet]
    public IActionResult GetAllBidsByUserId([FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            _logger.LogError("User ID cannot be empty");
            return BadRequest("User ID cannot be empty");
        }

        var bids = _biddingRepo.GetAllBidsByUserId(userId);
        return Ok(bids);
    }
    
    [Route("GetHighestBidByAuctionId")]
    [HttpGet]
    public IActionResult GetHighestBidByAuctionId([FromQuery] Guid auctionId)
    {
        if (auctionId == Guid.Empty)
        {
            _logger.LogError("Auction ID cannot be empty");
            return BadRequest("Auction ID cannot be empty");
        }

        var highestBid = _biddingRepo.GetHighestBidByAuctionId(auctionId);
        return Ok(highestBid);
    }
}
