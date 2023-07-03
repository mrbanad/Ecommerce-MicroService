using CommonClassLibrary.Dto.Comment.SubComment;

namespace CommonClassLibrary.Dto.Comment;

public class CommentDto : BaseDto
{
    public string Text { get; set; }

    public string TypeOf { get; set; }

    public string EntityId { get; set; }

    public bool IsRead { get; set; }

    public bool Anonymous { get; set; }

    public ulong Like { get; set; }

    public ulong Dislike { get; set; }

    public decimal? Rating { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateId { get; set; }

    public string? ActivatorId { get; set; }

    public ICollection<SubCommentDto> SubComments { get; set; }
}