using BlazorLearning.Api.Extensions;
using BlazorLearning.Api.Middleware;
using BlazorLearning.Api.Models;
using BlazorLearning.Api.Services;
using BlazorLearning.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSerilogServices(); // 添加 Serilog 服务
builder.Host.UseSerilog();
// 注册Mapster映射服务
builder.Services.AddMapsterService();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IJwtService, JwtService>(); // 添加 JWT 服务

//获取Jwt配置
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthentication();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // 禁用默认的模型验证错误响应
    options.SuppressModelStateInvalidFilter = true;
});

//添加FreeSql服务
builder.Services.AddFreeSqlService(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>(); // 添加全局异常处理中间件

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("我的 API 文档")
               .WithTheme(ScalarTheme.BluePlanet) // 或其他主题
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });// 添加 Scalar API 文档

    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    //    options.RoutePrefix = ""; // 设置根路径访问
    //});// 添加 Swagger UI
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 使用身份验证中间件
app.UseAuthorization();

app.MapControllers();

app.Run();