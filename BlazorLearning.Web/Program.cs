using BlazorLearning.Shared.ApiServices;
using BlazorLearning.Web.Components;
using BlazorLearning.Web.Handlers;
using BlazorLearning.Web.Services;
using MudBlazor.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// 注册 HttpClient 和 Refit
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7157";

// 1. 首先注册基础服务
builder.Services.AddScoped<ITokenService, TokenService>();

// 2. 注册认证消息处理器
builder.Services.AddScoped<AuthHttpMessageHandler>();

// 3. 注册带认证的Refit客户端
builder.Services.AddRefitClient<IUserApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<AuthHttpMessageHandler>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();