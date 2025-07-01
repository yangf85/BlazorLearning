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

builder.Services.AddSerilogServices(); // ��� Serilog ����
builder.Host.UseSerilog();
// ע��Mapsterӳ�����
builder.Services.AddMapsterService();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IJwtService, JwtService>(); // ��� JWT ����

//��ȡJwt����
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
    // ����Ĭ�ϵ�ģ����֤������Ӧ
    options.SuppressModelStateInvalidFilter = true;
});

//���FreeSql����
builder.Services.AddFreeSqlService(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>(); // ���ȫ���쳣�����м��

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("�ҵ� API �ĵ�")
               .WithTheme(ScalarTheme.BluePlanet) // ����������
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });// ��� Scalar API �ĵ�

    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    //    options.RoutePrefix = ""; // ���ø�·������
    //});// ��� Swagger UI
}

app.UseHttpsRedirection();

app.UseAuthentication(); // ʹ�������֤�м��
app.UseAuthorization();

app.MapControllers();

app.Run();