using BlazorLearning.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//���FreeSql����
builder.Services.AddFreeSqlService(builder.Configuration);

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();