namespace CommonClassLibrary.Dto;

public class BaseSearchDto : IBaseSearchDto
{
    public int Top { get; set; }

    public int Skip { get; set; }

    public bool? Action { get; set; }

    public string? Creator { get; set; }

    public string? Editor { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}