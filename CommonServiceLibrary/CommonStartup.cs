using System.IdentityModel.Tokens.Jwt;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Repository;
using CommonServiceLibrary.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace CommonServiceLibrary
{
    public static class CommonStartup
    {
        public static void BuilderConfigure(this WebApplicationBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton(typeof(BaseRepository<>));
            builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();
                var settings = ProjectSettings.RabbitMqSetting;

                var factory = new ConnectionFactory
                {
                    HostName = settings.HostName,
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(settings.UserName))
                    factory.UserName = settings.UserName;

                if (!string.IsNullOrEmpty(settings.Password))
                    factory.Password = settings.Password;

                var retryCount = 5;
                if (!string.IsNullOrEmpty(settings.RetryCount))
                    retryCount = int.Parse(settings.RetryCount);

                return new RabbitMqPersistentConnection(factory, logger, retryCount);
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/NewsSignalR") ||
                                 path.StartsWithSegments("/CategorySignalR")))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy =>
                {
                    // policy.RequireClaim("Access", "Admin", "System");
                    policy.RequireRole("Admin", "System");
                });
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

                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{builder.Environment.ApplicationName}.xml"));
            });
        }

        public static void Configure(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.MapControllers();
        }
    }
}