using CommonServiceLibrary;
using NewsServices.Config;
using NewsServices.RabbitMQ;
using NewsServices.Repository;
using NewsServices.Service;

var builder = WebApplication.CreateBuilder(args);

builder.BuilderConfigure();

builder.Services.AddSingleton<NewsRepository>();
builder.Services.AddSingleton<NewsService>();

MapsterConfig.ConfigureMappings();

builder.Services.AddHostedService<RabbitMqEvent>();

var app = builder.Build();

app.Configure();

app.Run();