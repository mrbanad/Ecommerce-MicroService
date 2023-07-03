using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommonClassLibrary.Model;

public class BaseModel : IBaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public bool Active { get; set; }

    public bool Delete { get; set; }

    public string Creator { get; set; }

    public string? Editor { get; set; }

    [BsonIgnore] public DateTimeOffset CreateDate => new ObjectId(Id).CreationTime;
}