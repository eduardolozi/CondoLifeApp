using System.Globalization;
using Application;
using Infraestructure;
using Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
var defaultCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
builder.Services.AddInfraServices();
builder.Services.AddApplicationServices();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddHostedService<NotificationBackgroundService>();
var host = builder.Build();
host.Run();
