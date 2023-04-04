using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace AuthenticationService.Model;

public class ApplicationRole : MongoIdentityRole<ObjectId>
{
}