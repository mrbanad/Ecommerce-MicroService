namespace CommonClassLibrary.Model;

public class BaseWithDisplayModel : BaseModel, IBaseWithDisplayModel
{
    public ICollection<DisplayModel> DisplayItems { get; set; }

    public ICollection<string>? ImageName { get; set; }
}