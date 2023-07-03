namespace CommonClassLibrary.Model;

public interface IBaseWithDisplayModel : IBaseModel
{
    public ICollection<DisplayModel> DisplayItems { get; set; }

    public ICollection<string> ImageName { get; set; }
}