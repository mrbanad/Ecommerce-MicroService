using CommonClassLibrary.Model;
using MongoDB.Bson;

namespace CommentServices.Model;

public class SubComment : BaseModel
{
    public SubComment(string text)
    {
        Text = text;
    }

    public string Text { get; set; }

    public ObjectId ParentId { get; set; }

    public bool IsRead { get; set; }

    public bool Anonymous { get; set; }

    public ulong Like { get; set; }

    public ulong Dislike { get; set; }

    public decimal? Rating { get; set; }

    public ICollection<SubComment>? SubComments { get; set; }

    public string? Activator { get; set; }
}