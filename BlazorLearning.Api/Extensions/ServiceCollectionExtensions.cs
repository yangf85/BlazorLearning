using BlazorLearning.Api.Services;
using Npgsql;

namespace BlazorLearning.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFreeSqlService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.PostgreSQL, connectionString)
            .UseMonitorCommand(cmd => Console.WriteLine($"执行 SQL: {cmd.CommandText}"))
            .UseAutoSyncStructure(true) // 自动同步数据库结构
            .Build();

        services.AddSingleton(fsql);

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}