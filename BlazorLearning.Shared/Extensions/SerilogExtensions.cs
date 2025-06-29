using BlazorLearning.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorLearning.Shared.Extensions;

/// <summary>
/// Serilog 配置扩展
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// 添加 Serilog 配置
    /// </summary>
    public static IServiceCollection AddSerilogServices(this IServiceCollection services)
    {
        // 配置 Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/blazor-api-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // 注册日志服务
        services.AddScoped<ILoggerService, LoggerService>();

        return services;
    }
}