using Application;
using Infraestructure;
using Shared;
using Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddSingleton<IServiceProvider>(sp => sp);
builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
var host = builder.Build();
host.Run();
