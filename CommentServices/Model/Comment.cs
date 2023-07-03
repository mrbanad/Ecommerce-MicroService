using CommonClassLibrary.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommentServices.Model;

public class Comment : BaseModel
{
    public Comment(string text, string entityType)
    {
        Text = text;
        TypeOf = entityType;
    }

    public string Text { get; set; }

    public string TypeOf { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId EntityId { get; set; }

    public bool IsRead { get; set; }

    public bool Anonymous { get; set; }

    public ulong Like { get; set; }

    public ulong Dislike { get; set; }

    public decimal? Rating { get; set; }

    public ICollection<SubComment>? SubComments { get; set; }

    public string? Activator { get; set; }
}