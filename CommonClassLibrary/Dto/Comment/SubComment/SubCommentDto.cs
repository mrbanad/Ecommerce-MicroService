namespace CommonClassLibrary.Dto.Comment.SubComment;

public class SubCommentDto : BaseDto
{
    public string Text { get; set; }

    public bool IsRead { get; set; }

    public bool Anonymous { get; set; }

    public ulong Like { get; set; }

    public ulong Dislike { get; set; }

    public decimal? Rating { get; set; }

    public DateTime CreateDate { get; set; }

    public string ParentId { get; set; }

    public string CreateId { get; set; }

    public ICollection<SubCommentDto> SubComments { get; set; }

    public string? ActivatorId { get; set; }
}