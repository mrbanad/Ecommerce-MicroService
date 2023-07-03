using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace AuthenticationServices.Model;

public class ApplicationRole : MongoIdentityRole<ObjectId>
{
    public string Title { get; set; }

    public string? Description { get; set; }

    public bool Delete { get; set; }

    public bool Active { get; set; }
}