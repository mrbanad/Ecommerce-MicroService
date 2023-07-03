using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommonClassLibrary.Dto;

public class BaseDto : IBaseDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public bool Active { get; set; }
}