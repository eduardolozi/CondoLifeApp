using System.Globalization;
using API;
using API.Hubs;
using Application;
using Infraestructure;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfraServices();
builder.Services.AddCors(options => {
	options.AddPolicy("AllowBlazorClient",
		builder => {
			builder.WithOrigins("https://localhost:7136")
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});
var app = builder.Build();

var defaultCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var localizationOptions = new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture(defaultCulture),
	SupportedCultures = new[] { defaultCulture },
	SupportedUICultures = new[] { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
