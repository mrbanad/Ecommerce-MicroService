using CategoryServices.Config;
using CategoryServices.RabbitMQ;
using CategoryServices.Repository;
using CategoryServices.Service;
using CommonServiceLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.BuilderConfigure();

builder.Services.AddSingleton<CategoryRepository>();
builder.Services.AddSingleton<CategoryService>();

MapsterConfig.ConfigureMappings();

builder.Services.AddHostedService<RabbitMqEvent>();

var app = builder.Build();

app.Configure();

app.Run();