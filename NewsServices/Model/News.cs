using CommonClassLibrary.Model;
using MongoDB.Bson;

namespace NewsServices.Model;

public class News : BaseWithDisplayModel
{
    public bool CanComment { get; set; }

    public ObjectId CategoryId { get; set; }

    public List<ObjectId>? RelatedNews { get; set; }
}