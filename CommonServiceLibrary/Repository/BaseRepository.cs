using System.Linq.Expressions;
using CommonClassLibrary.Model;
using CommonServiceLibrary.Setting;
using MongoDB.Driver;

namespace CommonServiceLibrary.Repository;

public class BaseRepository<T> where T : IBaseModel
{
    protected IMongoCollection<T> Collection;

    public BaseRepository()
    {
        var mongoClient = new MongoClient(ProjectSettings.DatabaseSetting.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(ProjectSettings.DatabaseSetting.DatabaseName);
        Collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
    }

    public virtual async Task<IEnumerable<T>> GetAsync(int top, int skip, CancellationToken cancellationToken)
    {
        var query = Collection.Find(_ => true);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> GetAsync(string id, CancellationToken cancellationToken) =>
        await Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, int top, int skip, CancellationToken cancellationToken)
    {
        var query = Collection.Find(predicate);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task CreateAsync(T newItem, CancellationToken cancellationToken) =>
        await Collection.InsertOneAsync(newItem, cancellationToken: cancellationToken);

    public virtual async Task UpdateAsync(T updatedItem, CancellationToken cancellationToken) =>
        await Collection.ReplaceOneAsync(x => x.Id == updatedItem.Id, updatedItem, cancellationToken: cancellationToken);

    public virtual async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await Collection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);
}