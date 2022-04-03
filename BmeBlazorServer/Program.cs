global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
using MudBlazor.Services;
using BmeBlazorServer.Services;
using BmeBlazorServer.Handlers;
using BmeBlazorServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMudServices();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<ValidateHeaderHandler>();
builder.Services.AddMudBlazorDialog();
builder.Services.AddAuthorizationCore();

/* AuthService as Singleton - inject WebAPI_URL */
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
});

builder.Services.AddHttpClient<ITransactionRepository, TransactionRepository>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
}).AddHttpMessageHandler<ValidateHeaderHandler>();

builder.Services.AddHttpClient<IUserRepository, UserRepository>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
}).AddHttpMessageHandler<ValidateHeaderHandler>();

builder.Services.AddHttpClient<ICategoryRepository, CategoryRepository>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("WebAPI_URL"));
}).AddHttpMessageHandler<ValidateHeaderHandler>();


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IOverviewService, OverviewService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

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
