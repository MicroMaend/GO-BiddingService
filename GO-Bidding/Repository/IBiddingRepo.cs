using GOCore;

namespace GO_Bidding;

public interface IBiddingRepo
{
    public Task PlaceBid(Bidding bid);
    
    public Task DeleteBid(Bidding bid);
    
    public List<Bidding> GetAllBidsByAuctionId(Guid auctionId);
    
    public List<Bidding> GetAllBidsByUserId(Guid userId);
    
    public Bidding GetHighestBidByAuctionId(Guid auctionId);
}