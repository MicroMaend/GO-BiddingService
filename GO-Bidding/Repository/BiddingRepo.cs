using GO_Bidding.Controllers;
using GOCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GO_Bidding;

public class BiddingRepo : IBiddingRepo
{
    private readonly IMongoCollection<Bidding> _biddingCollection;
    private readonly ILogger<BiddingRepo> _logger;

    public BiddingRepo(string connectionString, ILogger<BiddingRepo> logger)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string must be provided", nameof(connectionString));

        var databaseName = "GO-BiddingServiceDB";
        var collectionName = "Bidding";

        var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
        var client = new MongoClient(clientSettings);
        var database = client.GetDatabase(databaseName);
        _biddingCollection = database.GetCollection<Bidding>(collectionName);

    }

    public async Task PlaceBid(Bidding bid)
    {
        await _biddingCollection.InsertOneAsync(bid);
    }

    public async Task DeleteBid(Bidding bid)
    {
        var filter = Builders<Bidding>.Filter.Eq(b => b.Id, bid.Id);
        await _biddingCollection.DeleteOneAsync(filter);
    }

    public List<Bidding> GetAllBidsByAuctionId(Guid auctionId)
    {
        var filter = Builders<Bidding>.Filter.Eq(b => b.AuctionId, auctionId);
        return _biddingCollection.Find(filter).ToList();
    }

    public List<Bidding> GetAllBidsByUserId(Guid userId)
    {
        var filter = Builders<Bidding>.Filter.Eq(b => b.UserId, userId);
        return _biddingCollection.Find(filter).ToList();
    }

    public Bidding? GetHighestBidByAuctionId(Guid auctionId)
    {
        _logger.LogInformation("Looking for bids with AuctionId: {AuctionId}", auctionId);

        var filter = Builders<Bidding>.Filter.Eq(b => b.AuctionId, auctionId);
        var sort = Builders<Bidding>.Sort.Descending(b => b.Amount);

        var result = _biddingCollection.Find(filter).Sort(sort).FirstOrDefault();

        if (result == null)
            _logger.LogWarning("No bids found for AuctionId: {AuctionId}", auctionId);
        else
            _logger.LogInformation("Highest bid found: {BidId} with amount {Amount}", result.Id, result.Amount);

        return result;
    }

}
