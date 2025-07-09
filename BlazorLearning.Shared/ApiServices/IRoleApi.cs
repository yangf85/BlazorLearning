using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Dtos;
using Refit;

namespace BlazorLearning.Shared.ApiServices;

/// <summary>
/// 角色管理API接口定义
/// </summary>
public interface IRoleApi
{
    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    [Get("/api/roles")]
    Task<ApiResult<List<RoleDto>>> GetAllRolesAsync();

    /// <summary>
    /// 根据ID获取角色详情
    /// </summary>
    /// <param name="id">角色ID</param>
    [Get("/api/roles/{id}")]
    Task<ApiResult<RoleDto>> GetRoleByIdAsync(int id);

    /// <summary>
    /// 创建新角色
    /// </summary>
    /// <param name="request">创建角色请求</param>
    [Post("/api/roles")]
    Task<ApiResult<RoleDto>> CreateRoleAsync([Body] CreateRoleRequest request);

    /// <summary>
    /// 更新角色信息
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="request">更新角色请求</param>
    [Put("/api/roles/{id}")]
    Task<ApiResult<RoleDto>> UpdateRoleAsync(int id, [Body] UpdateRoleRequest request);

    /// <summary>
    /// 删除角色（软删除）
    /// </summary>
    /// <param name="id">角色ID</param>
    [Delete("/api/roles/{id}")]
    Task<ApiResult<string>> DeleteRoleAsync(int id);
}