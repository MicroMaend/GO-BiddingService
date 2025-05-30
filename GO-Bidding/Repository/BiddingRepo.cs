using GOCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GO_Bidding;

public class BiddingRepo : IBiddingRepo
{
    private readonly IMongoCollection<Bidding> _biddingCollection;

    public BiddingRepo(string connectionString)
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

    public Bidding GetHighestBidByAuctionId(Guid auctionId)
    {
        var filter = Builders<Bidding>.Filter.Eq(b => b.AuctionId, auctionId);
        var sort = Builders<Bidding>.Sort.Descending(b => b.Amount);
        return _biddingCollection.Find(filter).Sort(sort).FirstOrDefault();
    }
}
