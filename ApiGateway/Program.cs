using ApiGateway;
using ApiGateway.Middleware;
using ApiGateway.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ProductService.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();

    var setting = new RabbitMqSettings();
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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("IdentityApiKey", options =>
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
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ExceptionHandlingConfiguration.OnJwtAuthenticationFailed,
            OnForbidden = ExceptionHandlingConfiguration.OnJwtForbidden,
            OnChallenge = ExceptionHandlingConfiguration.OnJwtChallenge
        };
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



builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseSwaggerForOcelotUI(opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; }).UseOcelot().Wait();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();