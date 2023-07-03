namespace CommonClassLibrary.Dto.Category;

public class CategoryDto : BaseDto
{
    public string Title { get; set; }

    public string? Description { get; set; }

    public string TypeOf { get; set; }

    public string EntityId { get; set; }
}