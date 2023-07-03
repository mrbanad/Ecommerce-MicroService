using CommonServiceLibrary.Setting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthenticationServices.Model.Setting;

public class PhoneConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
{
    /// <summary>
    /// Logger
    /// </summary>
    private readonly ILogger<PhoneConfirmationTokenProvider<TUser>> _logger;

    /// <summary>
    /// UserCollection
    /// </summary>
    private readonly IMongoCollection<ApplicationUser> _userCollection;

    public PhoneConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
        IOptions<PhoneConfirmationTokenProviderOptions> options,
        ILogger<PhoneConfirmationTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(ProjectSettings.DatabaseSetting.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(
            ProjectSettings.DatabaseSetting.DatabaseName);
        _userCollection = mongoDatabase.GetCollection<ApplicationUser>(
            "applicationUsers");
    }

    public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var correctUser = user as ApplicationUser;

        var generator = new Random();
        var token = generator.Next(0, 1000000).ToString("D6");

        var filter = Builders<ApplicationUser>.Filter.Eq(x => x.Id, ObjectId.Parse(correctUser?.Id.ToString()));
        var update = Builders<ApplicationUser>.Update
            .Set(x => x.ActivationCode, token)
            .Set(x => x.ExpireActivationCode, DateTime.UtcNow.AddMinutes(2));

        await _userCollection.UpdateOneAsync(filter, update);

        return token;
    }

    public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var correctUser = user as ApplicationUser;

        var result = correctUser?.ActivationCode == token && DateTime.UtcNow < correctUser.ExpireActivationCode;

        if (!result)
            return result;

        var filter = Builders<ApplicationUser>.Filter.Eq(x => x.Id, ObjectId.Parse(correctUser?.Id.ToString()));
        var update = Builders<ApplicationUser>.Update
            .Set(x => x.ActivationCode, null)
            .Set(x => x.ExpireActivationCode, null);

        await _userCollection.UpdateOneAsync(filter, update);
        return result;
    }
}

public class PhoneConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
}