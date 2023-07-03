using CommonClassLibrary.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CategoryServices.Model;

public class SubCategory : BaseWithDisplayModel
{
    public SubCategory()
    {
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId ParentId { get; set; }

    public ICollection<SubCategory>? SubCategories { get; set; }
}