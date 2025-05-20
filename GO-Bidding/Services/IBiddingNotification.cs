using GOCore;

namespace GO_Bidding.Services;

public interface IBiddingNotification
{
    Task SendBidding(Bidding bidding);
}