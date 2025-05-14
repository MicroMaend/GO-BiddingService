using Microsoft.AspNetCore.Mvc;
using GOCore;
namespace GO_Bidding.Controllers;

[ApiController]
[Route("[controller]")]
public class BiddingController : ControllerBase
{

    private readonly ILogger<BiddingController> _logger;
    private readonly IBiddingRepo _biddingRepo;

    public BiddingController(ILogger<BiddingController> logger, IBiddingRepo biddingRepo)
    {
        _biddingRepo = biddingRepo;
        _logger = logger;
    }
    

    [Route("PlaceBid")]
    [HttpPost]
    public IActionResult PlaceBid([FromBody] Bidding bid)
    {
        if (bid == null)
        {
            _logger.LogError("Bid cannot be null");
            return BadRequest("Bid cannot be null");
        }

        _biddingRepo.PlaceBid(bid);
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
