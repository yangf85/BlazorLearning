using BlazorLearning.Shared.Dtos;
using BlazorLearning.Shared.Models;
using Refit;

namespace BlazorLearning.Shared.ApiServices;

/// <summary>
/// 用户角色分配API接口 - 支持完整的用户角色关系管理
/// </summary>
public interface IUserRoleApi
{
    /// <summary>
    /// 为用户分配角色（支持批量分配）
    /// </summary>
    /// <param name="request">角色分配请求</param>
    /// <returns>分配结果</returns>
    [Post("/api/user-roles/assign")]
    Task<ApiResult<object>> AssignRolesAsync([Body] AssignRoleRequest request);

    /// <summary>
    /// 取消用户的角色（支持批量取消）
    /// </summary>
    /// <param name="request">角色取消请求</param>
    /// <returns>取消结果</returns>
    [Post("/api/user-roles/unassign")]
    Task<ApiResult<object>> UnassignRolesAsync([Body] UnassignRoleRequest request);

    /// <summary>
    /// 替换用户的所有角色（原子性操作）
    /// </summary>
    /// <param name="request">角色替换请求</param>
    /// <returns>替换结果</returns>
    [Post("/api/user-roles/replace")]
    Task<ApiResult<object>> ReplaceUserRolesAsync([Body] ReplaceUserRolesRequest request);

    /// <summary>
    /// 获取指定用户的所有角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户角色信息</returns>
    [Get("/api/user-roles/user/{userId}/roles")]
    Task<ApiResult<UserRoleResponse>> GetUserRolesAsync(int userId);

    /// <summary>
    /// 获取指定角色的所有用户
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>角色用户信息</returns>
    [Get("/api/user-roles/role/{roleId}/users")]
    Task<ApiResult<RoleUserResponse>> GetRoleUsersAsync(int roleId);

    /// <summary>
    /// 检查用户是否拥有指定角色
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="roleId">角色ID</param>
    /// <returns>是否拥有角色</returns>
    [Get("/api/user-roles/check/{userId}/{roleId}")]
    Task<ApiResult<bool>> CheckUserRoleAsync(int userId, int roleId);

    /// <summary>
    /// 获取用户角色分配详情（支持按用户或角色筛选）
    /// </summary>
    /// <param name="userId">用户ID（可选）</param>
    /// <param name="roleId">角色ID（可选）</param>
    /// <returns>用户角色详情列表</returns>
    [Get("/api/user-roles/details")]
    Task<ApiResult<List<UserRoleDetailResponse>>> GetUserRoleDetailsAsync([Query] int? userId = null, [Query] int? roleId = null);
}