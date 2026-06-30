using LoggingApi.Blazor.Components;
using LoggingApi.Blazor.Services;
using LoggingApi.Blazor.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<AuthClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseAddress"]));

builder.Services.AddHttpClient<LogsClient>(client =>
        client.BaseAddress = new Uri(builder.Configuration["Api:BaseAddress"]))
    .AddHttpMessageHandler<JwtHandler>();

builder.Services.AddLocalStorageServices();
builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddTransient<JwtHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();