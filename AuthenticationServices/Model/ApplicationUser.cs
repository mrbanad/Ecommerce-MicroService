using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace AuthenticationServices.Model;

public class ApplicationUser : MongoIdentityUser<ObjectId>
{
    public decimal Wallet { get; set; }

    public DateTime? ExpireActivationCode { get; set; }

    public string? ActivationCode { get; set; }

    public ICollection<ObjectId>? AddressId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? FatherName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? NationalCode { get; set; }

    public bool Delete { get; set; }
}