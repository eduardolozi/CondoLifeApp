using System.Globalization;
using API;
using API.Handlers;
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseMiddleware<AuthorizationResponseMiddleware>();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
