using BlazorApp.Components;
using BlazorApp.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMemoryCache();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<LocationService>((client) => {
    //env
    client.BaseAddress = new Uri("https://www.universal-tutorial.com/api/");
    client.DefaultRequestHeaders.Add("Accept", "*/*");
});

builder.Services.AddHttpClient<UserService>(client => {
	client.BaseAddress = new Uri("https://localhost:7031/api/User/");
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
