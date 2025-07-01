using BlazorLearning.Api.Models;
using BlazorLearning.Shared.Models;
using Mapster;

namespace BlazorLearning.Api.Configurations;

/// <summary>
/// Mapster 对象映射配置
/// </summary>
public static class MapsterConfig
{
    /// <summary>
    /// 配置映射规则
    /// </summary>
    public static void Configure()
    {
        // CreateRoleRequest 到 Role 的映射
        TypeAdapterConfig<CreateRoleRequest, Role>
            .NewConfig()
            .Map(dest => dest.IsActive, src => true)                    // 新建角色默认激活
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)        // 自动设置创建时间
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow);       // 自动设置更新时间

        // UpdateRoleRequest 到 Role 的映射
        TypeAdapterConfig<UpdateRoleRequest, Role>
            .NewConfig()
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)        // 自动设置更新时间
            .Ignore(dest => dest.Id)                                    // 保持原有ID
            .Ignore(dest => dest.IsActive)                              // 保持原有状态
            .Ignore(dest => dest.CreatedAt);                            // 保持原有创建时间

        // Role 到 RoleDto 完全自动映射，字段名相同
    }
}