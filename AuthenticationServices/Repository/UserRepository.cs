using System.Linq.Expressions;
using AuthenticationServices.Model;
using CommonServiceLibrary.Setting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthenticationServices.Repository;

public class UserRepository
{
    /// <summary>
    ///  ApplicationUserCollection
    /// </summary>
    private readonly IMongoCollection<ApplicationUser> _userCollection;

    public UserRepository()
    {
        var mongoClient = new MongoClient(
            ProjectSettings.DatabaseSetting.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ProjectSettings.DatabaseSetting.DatabaseName);

        _userCollection = mongoDatabase.GetCollection<ApplicationUser>(
            "applicationUsers");
    }

    public async Task<IEnumerable<ApplicationUser>> GetAsync(int top, int skip, CancellationToken cancellationToken)
    {
        var query = _userCollection.Find(_ => true);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<ApplicationUser> GetAsync(string id, CancellationToken cancellationToken) =>
        await _userCollection.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<ApplicationUser>> GetAsync(Expression<Func<ApplicationUser, bool>> predicate, int top, int skip, CancellationToken cancellationToken)
    {
        var query = _userCollection.Find(predicate);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(ApplicationUser newUser, CancellationToken cancellationToken) =>
        await _userCollection.InsertOneAsync(newUser, cancellationToken: cancellationToken);

    public async Task UpdateAsync(ApplicationUser updatedUser, CancellationToken cancellationToken) =>
        await _userCollection.ReplaceOneAsync(x => x.Id == updatedUser.Id, updatedUser, cancellationToken: cancellationToken);

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _userCollection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id), cancellationToken: cancellationToken);
}