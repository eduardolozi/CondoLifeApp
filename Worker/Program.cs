using Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<EmailBackgroundService>();

var host = builder.Build();
host.Run();
