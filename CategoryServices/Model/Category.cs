using CommonClassLibrary.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CategoryServices.Model;

public class Category : BaseWithDisplayModel
{
    public Category(string typeOf)
    {
        TypeOf = typeOf;
    }

    public string TypeOf { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId EntityId { get; set; }

    public ICollection<SubCategory>? SubCategories { get; set; }
}