using API;
using API.Handlers;
using API.Hubs;
using Application;
using Infraestructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfraServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

//app.UseExceptionHandler();

app.MapControllers();

app.MapHub<EmailNotificationHub>("/emailNotificationHub");

app.Run();
