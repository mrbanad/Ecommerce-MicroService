using System.Reflection;
using AuthenticationServices.Config;
using AuthenticationServices.Model;
using AuthenticationServices.Model.Setting;
using AuthenticationServices.Repository;
using AuthenticationServices.Service;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(ProjectSettings.DatabaseSetting.ConnectionString));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<RoleRepository>();
builder.Services.AddSingleton<RoleService>();

MapsterConfig.ConfigureMappings();

builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();

    var setting = ProjectSettings.RabbitMqSetting;
    builder.Configuration.GetSection("RabbitMq").Bind(setting);

    var factory = new ConnectionFactory
    {
        HostName = setting.HostName,
        DispatchConsumersAsync = true
    };

    if (!string.IsNullOrEmpty(setting.UserName))
        factory.UserName = setting.UserName;

    if (!string.IsNullOrEmpty(setting.Password))
        factory.Password = setting.Password;

    var retryCount = 5;
    if (!string.IsNullOrEmpty(setting.RetryCount))
        retryCount = int.Parse(setting.RetryCount);

    return new RabbitMqPersistentConnection(factory, logger, retryCount);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(identityOptions =>
    {
        identityOptions.Password.RequireDigit = true;
        identityOptions.Password.RequireLowercase = true;
        identityOptions.Password.RequireNonAlphanumeric = false;
        identityOptions.Password.RequireUppercase = true;
        identityOptions.Password.RequiredLength = 8;
        identityOptions.User.RequireUniqueEmail = true;
        identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        identityOptions.Lockout.MaxFailedAccessAttempts = 5;
        identityOptions.Lockout.AllowedForNewUsers = true;
        identityOptions.Tokens.ChangePhoneNumberTokenProvider = "PhoneConfirmation";
    })
    .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(ProjectSettings.DatabaseSetting.ConnectionString,
        ProjectSettings.DatabaseSetting.DatabaseName)
    .AddDefaultTokenProviders()
    .AddTokenProvider<PhoneConfirmationTokenProvider<ApplicationUser>>("PhoneConfirmation");

builder.Services.Configure<PhoneConfirmationTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(2);
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = "https://localhost:4200",
            ValidIssuer = "https://localhost",
            IssuerSigningKey = new SymmetricSecurityKey("sadawdwdawdadsdawdasdawdaxdwadaxcxcxcxcssa"u8.ToArray())
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345TokenData'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    x.IncludeXmlComments(xmlPath);
});

builder.Services.AddSignalR();

builder.Services.AddCors(opt => opt.AddPolicy("CorsPolicy",
    corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
    }
));

builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();