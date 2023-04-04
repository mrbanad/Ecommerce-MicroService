using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketService.Model;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Title { get; set; }

    public int Price { get; set; }
}