using BlazorLearning.Api.Configurations;
using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using Mapster;
using MapsterMapper;
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
            .Build();

        fsql.CodeFirst.SyncStructure<User>();
        fsql.CodeFirst.SyncStructure<UserRole>();
        fsql.CodeFirst.SyncStructure<Role>();
        fsql.CodeFirst.SyncStructure<RolePermission>();

        services.AddSingleton(fsql);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }

    public static IServiceCollection AddMapsterService(this IServiceCollection services)
    {
        // 配置映射规则
        MapsterConfig.Configure();

        // 注册 Mapster 服务
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}