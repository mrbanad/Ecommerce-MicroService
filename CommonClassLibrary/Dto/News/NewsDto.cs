namespace CommonClassLibrary.Dto.News;

public class NewsDto : BaseDto
{
    public string Description { get; set; }

    public string Title { get; set; }

    public string Text { get; set; }

    public bool CanComment { get; set; }

    public string CategoryId { get; set; }

    public ICollection<string>? Images { get; set; }
}