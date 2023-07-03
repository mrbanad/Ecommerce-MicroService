using MongoDB.Bson;

namespace CommonClassLibrary.Model;

public interface IBaseModel
{
    public string Id { get; set; }

    public bool Active { get; set; }

    public bool Delete { get; set; }

    public string Creator { get; set; }

    public string? Editor { get; set; }

    public DateTimeOffset CreateDate => new ObjectId(Id).CreationTime;
}