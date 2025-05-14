using GOCore;

namespace GO_Bidding;

public interface IBiddingRepo
{
    public void PlaceBid(Bidding bid);
    
    public void DeleteBid(Bidding bid);
    
    public List<Bidding> GetAllBidsByAuctionId(Guid auctionId);
    
    public List<Bidding> GetAllBidsByUserId(Guid userId);
    
    public Bidding GetHighestBidByAuctionId(Guid auctionId);
}