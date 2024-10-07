using Application;
using Infraestructure;
using Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddHostedService<EmailBackgroundService>();
var host = builder.Build();
host.Run();
