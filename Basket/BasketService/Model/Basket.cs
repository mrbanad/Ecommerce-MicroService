using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BasketService.Model;

public class Basket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public List<BasketItem> Items { get; set; }
}