using System.Globalization;
using BlazorApp.Components;
using BlazorApp.Services;
using BlazorApp.Utils;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMemoryCache();
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<PageHelper>();
builder.Services.AddHttpClient<LocationService>((client) => {
    //env
    client.BaseAddress = new Uri("https://www.universal-tutorial.com/api/");
    client.DefaultRequestHeaders.Add("Accept", "*/*");
});

builder.Services.AddHttpClient<UserService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/User/");
});
builder.Services.AddHttpClient<CondominiumService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Condominium/");
});
builder.Services.AddHttpClient<AuthService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Auth/");
});
builder.Services.AddHttpClient<SpaceService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Space/");
});
builder.Services.AddHttpClient<BookingService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Booking/");
});
builder.Services.AddHttpClient<NotificationService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Notification/");
});
builder.Services.AddHttpClient<VotingService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/Voting/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
