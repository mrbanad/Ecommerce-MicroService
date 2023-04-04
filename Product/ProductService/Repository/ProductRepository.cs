using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.Model;

namespace ProductService.Repository;

public class ProductRepository
{
    private readonly IMongoCollection<Product> _productCollection;

    public ProductRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _productCollection = mongoDatabase.GetCollection<Product>(
            nameof(Product));
    }

    public async Task<List<Product>> GetAsync(CancellationToken cancellationToken) =>
        await _productCollection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);

    public async Task<Product> GetAsync(string id, CancellationToken cancellationToken) =>
        await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<List<Product>> GetAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken) =>
        await _productCollection.Find(predicate).ToListAsync(cancellationToken: cancellationToken);

    public async Task CreateAsync(Product newProduct, CancellationToken cancellationToken) =>
        await _productCollection.InsertOneAsync(newProduct, cancellationToken: cancellationToken);

    public async Task UpdateAsync(string id, Product updatedProduct, CancellationToken cancellationToken) =>
        await _productCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct, cancellationToken: cancellationToken);

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _productCollection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);
}