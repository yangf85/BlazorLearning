using BlazorLearning.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//添加FreeSql服务
builder.Services.AddFreeSqlService(builder.Configuration);

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();