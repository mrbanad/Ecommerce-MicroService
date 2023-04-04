using System.Linq.Expressions;
using BasketService.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BasketService.Repository;

public class BasketRepository
{
    private readonly IMongoCollection<Basket> _basketCollection;

    public BasketRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _basketCollection = mongoDatabase.GetCollection<Basket>(
            nameof(Basket));
    }

    public async Task<List<Basket>> GetAsync(CancellationToken cancellationToken) =>
        await _basketCollection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);

    public async Task<Basket> GetAsync(string id, CancellationToken cancellationToken) =>
        await _basketCollection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<List<Basket>> GetAsync(Expression<Func<Basket, bool>> predicate, CancellationToken cancellationToken) =>
        await _basketCollection.Find(predicate).ToListAsync(cancellationToken: cancellationToken);

    public async Task CreateAsync(Basket newBasket, CancellationToken cancellationToken) =>
        await _basketCollection.InsertOneAsync(newBasket, cancellationToken: cancellationToken);

    public async Task UpdateAsync(string id, Basket updatedBasket, CancellationToken cancellationToken) =>
        await _basketCollection.ReplaceOneAsync(x => x.Id == id, updatedBasket, cancellationToken: cancellationToken);

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _basketCollection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);
}