using API.Hubs.Services;
using Application;
using Infraestructure;
using Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
var host = builder.Build();
host.Run();
