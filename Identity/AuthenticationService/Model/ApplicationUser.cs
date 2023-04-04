using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace AuthenticationService.Model;

public class ApplicationUser : MongoIdentityUser<ObjectId>
{
}