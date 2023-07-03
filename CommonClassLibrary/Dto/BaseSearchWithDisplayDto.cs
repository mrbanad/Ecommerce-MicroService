namespace CommonClassLibrary.Dto;

public class BaseSearchWithDisplayDto : BaseSearchDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ImageName { get; set; }

    public string? Keyword { get; set; }
}