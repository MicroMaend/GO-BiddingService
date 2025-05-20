using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using GO_Bidding.Controllers;
using GO_Bidding.Services;
using Microsoft.AspNetCore.Mvc;
using GOCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GO_Bidding;

namespace GO_BiddingTests
{
    [TestClass]
    public class BiddingControllerTests
    {
        private Mock<ILogger<BiddingController>> _loggerMock;
        private Mock<IBiddingRepo> _biddingRepoMock;
        private Mock<IBiddingNotification> _biddingNotificationMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BiddingController>>();
            _biddingRepoMock = new Mock<IBiddingRepo>();
            _biddingNotificationMock = new Mock<IBiddingNotification>();
        }

        private BiddingController GetController()
        {
            return new BiddingController(_loggerMock.Object, _biddingRepoMock.Object, _biddingNotificationMock.Object);
        }

        [TestMethod]
        public async Task PlaceBid_NullBid_ReturnsBadRequest()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = await controller.PlaceBid(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task PlaceBid_BidNotHigherThanCurrent_ReturnsBadRequest()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var bid = new Bidding { Amount = 100, AuctionId = auctionId };
            var existingBid = new Bidding { Amount = 100, AuctionId = auctionId };

            _biddingRepoMock.Setup(r => r.GetHighestBidByAuctionId(auctionId)).Returns(existingBid);

            var controller = GetController();

            // Act
            var result = await controller.PlaceBid(bid);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task PlaceBid_ValidBid_ReturnsOk()
        {
            // Arrange
            var bid = new Bidding
            {
                Id = Guid.NewGuid(),
                Amount = 200,
                AuctionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Date = DateTime.UtcNow
            };

            _biddingRepoMock.Setup(r => r.GetHighestBidByAuctionId(bid.AuctionId)).Returns((Bidding)null);
            _biddingRepoMock.Setup(r => r.PlaceBid(bid)).Returns(Task.CompletedTask);
            _biddingNotificationMock.Setup(n => n.SendBidding(bid)).Returns(Task.CompletedTask);

            var controller = GetController();

            // Act
            var result = await controller.PlaceBid(bid);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(bid, okResult.Value);
        }

        [TestMethod]
        public void DeleteBid_NullBid_ReturnsBadRequest()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = controller.DeleteBid(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void GetAllBidsByAuctionId_EmptyGuid_ReturnsBadRequest()
        {
            // Arrange
            var controller = GetController();

            // Act
            var result = controller.GetAllBidsByAuctionId(Guid.Empty);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}