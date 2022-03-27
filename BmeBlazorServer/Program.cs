global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
using MudBlazor.Services;
using BmeBlazorServer.Services;
using BmeBlazorServer.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<ValidateHeaderHandler>();

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
}).AddHttpMessageHandler<ValidateHeaderHandler>();

builder.Services.AddHttpClient<ITransactionService, TransactionService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
}).AddHttpMessageHandler<ValidateHeaderHandler>();

/* AuthService as Singleton - inject WebAPI_URL */
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
});

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
