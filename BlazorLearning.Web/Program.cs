using BlazorLearning.Shared.ApiServices;
using BlazorLearning.Shared.Extensions;
using BlazorLearning.Web.Components;
using BlazorLearning.Web.Handlers;
using BlazorLearning.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddSerilogServices();
// HttpClient��Refit����
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7157";

// 1. ע����֤��ط���
builder.Services.AddSingleton<CustomAuthStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

// 2. ע��Token����
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<AuthService>();

// 3. ע����֤��Ϣ������
builder.Services.AddScoped<AuthHttpMessageHandler>();

// 4. ע�������֤��Refit�ͻ���
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

builder.Services.AddRefitClient<IRoleApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddHttpMessageHandler<AuthHttpMessageHandler>();

builder.Services.AddRefitClient<IUserRoleApi>()
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