using System.ComponentModel.DataAnnotations;

namespace CommonClassLibrary.Model;

public class DisplayModel
{
    public DisplayModel(string description, string title)
    {
        Description = description;
        Title = title;
    }

    public string Title { get; set; }

    [MaxLength(2)] public string Language { get; set; }

    public string Description { get; set; }

    public string? HtmlContent { get; set; }

    public string? Header { get; set; }

    public ICollection<string>? Keyword { get; set; }
}