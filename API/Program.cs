using API;
using API.Handlers;
using API.Hubs;
using Application;
using Infraestructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Blazored.LocalStorage;

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
app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapHub<EmailNotificationHub>("/emailNotificationHub");

app.Run();
