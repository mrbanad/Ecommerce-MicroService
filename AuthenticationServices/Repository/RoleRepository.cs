using System.Linq.Expressions;
using AuthenticationServices.Model;
using CommonServiceLibrary.Setting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthenticationServices.Repository;

public class RoleRepository
{
    /// <summary>
    ///  ApplicationRoleCollection
    /// </summary>
    private readonly IMongoCollection<ApplicationRole> _roleCollection;

    public RoleRepository()
    {
        var mongoClient = new MongoClient(
            ProjectSettings.DatabaseSetting.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            ProjectSettings.DatabaseSetting.DatabaseName);

        _roleCollection = mongoDatabase.GetCollection<ApplicationRole>(
            "applicationRoles");
    }

    public async Task<IEnumerable<ApplicationRole>> GetAsync(int top, int skip, CancellationToken cancellationToken)
    {
        var query = _roleCollection.Find(_ => true);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<ApplicationRole> GetAsync(string id, CancellationToken cancellationToken) =>
        await _roleCollection.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<IEnumerable<ApplicationRole>> GetAsync(Expression<Func<ApplicationRole, bool>> predicate, int top, int skip, CancellationToken cancellationToken)
    {
        var query = _roleCollection.Find(predicate);

        if (skip != 0)
            query = query.Skip(skip);
        if (top != 0)
            query = query.Limit(top);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(ApplicationRole newRole, CancellationToken cancellationToken) =>
        await _roleCollection.InsertOneAsync(newRole, cancellationToken: cancellationToken);

    public async Task UpdateAsync(ApplicationRole updatedRole, CancellationToken cancellationToken) =>
        await _roleCollection.ReplaceOneAsync(x => x.Id == updatedRole.Id, updatedRole, cancellationToken: cancellationToken);

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _roleCollection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id), cancellationToken: cancellationToken);
}