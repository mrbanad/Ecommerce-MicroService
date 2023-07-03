using CommonServiceLibrary;
using CommentServices.Config;
using CommentServices.RabbitMQ;
using CommentServices.Repository;
using CommentServices.Service;

var builder = WebApplication.CreateBuilder(args);

builder.BuilderConfigure();

builder.Services.AddSingleton<CommentRepository>();
builder.Services.AddSingleton<CommentService>();

MapsterConfig.ConfigureMappings();

builder.Services.AddHostedService<RabbitMqEvent>();

var app = builder.Build();

app.Configure();

app.Run();