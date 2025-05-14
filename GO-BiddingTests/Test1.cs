using GO_Bidding;
using GOCore;
using Moq;

namespace GO_BiddingTests;

/* Få oprettet tests af disse metoder, med act assert og assest
 
 * CreateBid()
   DeleteBid()
   GetAllBidsByAuctionId()
   GetAllBidsByUserId()
   GetHighestBidByAuctionId()
 */
/*
[TestClass]
public class CreateBidTests
{
    [TestMethod]
    public void CreateBid_shouldReturnCorrectBid()
    {
        // Arrange
        var mockInterface = new Mock<IBiddingRepo>();
        var expectedBid = new Bidding(1000,new Guid());
        mockInterface.Setup(x => x.PlaceBid(It.IsAny<Bidding>())).Returns(expectedBid);

        var service = new BiddingService(mockInterface.Object);

        // Act
        var result = service.CreateBid(new Bidding { Amount = 100 });

        // Assert
        Assert.AreEqual(expectedBid.Amount, result.Amount);
        Assert.AreEqual(expectedBid.Id, result.Id);

        // Verify
        mockInterface.Verify(x => x.CreateBid(It.IsAny<Bidding>()), Times.Once);
    }
}*/