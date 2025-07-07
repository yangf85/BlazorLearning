using BlazorLearning.Shared.Dtos;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[Route("api/permissions")]
[Authorize]
public class PermissionController : BaseApiController
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    // 检查指定用户是否拥有某个角色
    [HttpPost("check")]
    public async Task<ActionResult<ApiResult<PermissionCheckResponse>>> CheckPermission(
        [FromBody] PermissionCheckRequest request)
    {
        try
        {
            var userId = request.UserId ?? GetCurrentUserId();
            if (userId == null)
            {
                return BadRequest(ApiResult<PermissionCheckResponse>.FailResult("无效的用户信息"));
            }

            var hasRole = await _permissionService.HasRoleAsync(userId.Value, request.RoleName);
            var currentUserId = GetCurrentUserId();
            var currentUsername = GetCurrentUsername();

            var response = new PermissionCheckResponse
            {
                UserId = userId.Value,
                Username = currentUsername ?? "Unknown",
                RoleName = request.RoleName,
                HasRole = hasRole,
                CheckedAt = DateTime.UtcNow
            };

            return Ok(ApiResult<PermissionCheckResponse>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult<PermissionCheckResponse>.FailResult($"权限检查失败: {ex.Message}"));
        }
    }

    // 获取当前用户权限概览
    [HttpGet("overview")]
    public async Task<ActionResult<ApiResult<UserPermissionOverview>>> GetCurrentUserPermissions()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(ApiResult<UserPermissionOverview>.FailResult("未登录"));
            }

            var overview = await _permissionService.GetUserPermissionOverviewAsync(currentUserId.Value);
            if (overview == null)
            {
                return NotFound(ApiResult<UserPermissionOverview>.FailResult("用户不存在"));
            }

            return Ok(ApiResult<UserPermissionOverview>.SuccessResult(overview));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult<UserPermissionOverview>.FailResult($"获取权限概览失败: {ex.Message}"));
        }
    }

    // 获取指定用户权限概览
    [HttpGet("overview/{userId}")]
    public async Task<ActionResult<ApiResult<UserPermissionOverview>>> GetUserPermissions(int userId)
    {
        try
        {
            // 只有管理员才能查看其他用户的权限
            var isAdmin = await _permissionService.CurrentUserHasRoleAsync("Admin");
            var currentUserId = GetCurrentUserId();

            if (!isAdmin && currentUserId != userId)
            {
                return Forbid();
            }

            var overview = await _permissionService.GetUserPermissionOverviewAsync(userId);
            if (overview == null)
            {
                return NotFound(ApiResult<UserPermissionOverview>.FailResult("用户不存在"));
            }

            return Ok(ApiResult<UserPermissionOverview>.SuccessResult(overview));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult<UserPermissionOverview>.FailResult($"获取权限概览失败: {ex.Message}"));
        }
    }

    // 检查当前用户是否拥有指定角色
    [HttpGet("has-role/{roleName}")]
    public async Task<ActionResult<ApiResult<bool>>> HasRole(string roleName)
    {
        try
        {
            var hasRole = await _permissionService.CurrentUserHasRoleAsync(roleName);
            return Ok(ApiResult<bool>.SuccessResult(hasRole));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult<bool>.FailResult($"角色检查失败: {ex.Message}"));
        }
    }

    // 检查当前用户是否为管理员
    [HttpGet("is-admin")]
    public async Task<ActionResult<ApiResult<bool>>> IsAdmin()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(ApiResult<bool>.FailResult("未登录"));
            }

            var isAdmin = await _permissionService.IsAdminAsync(currentUserId.Value);
            return Ok(ApiResult<bool>.SuccessResult(isAdmin));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResult<bool>.FailResult($"管理员检查失败: {ex.Message}"));
        }
    }
}