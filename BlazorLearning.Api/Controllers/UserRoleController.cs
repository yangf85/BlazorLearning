using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Dtos;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[ApiController]
[Route("api/user-roles")]
[Authorize]
public class UserRoleController : BaseApiController
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILoggerService _logger;

    public UserRoleController(IUserRoleRepository userRoleRepository, ILoggerService logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRoleRequest request)
    {
        if (request.UserId <= 0 || !request.RoleIds.Any())
        {
            return BadRequest(ApiResult<object>.FailResult("用户ID和角色ID不能为空"));
        }

        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Unauthorized(ApiResult<object>.FailResult("无效的用户信息"));
        }

        var success = await _userRoleRepository.AssignRolesToUserAsync(
            request.UserId,
            request.RoleIds,
            currentUserId.Value);

        if (success)
        {
            _logger.Information("用户 {CurrentUserId} 成功为用户 {UserId} 分配角色", currentUserId, request.UserId);
            return Ok(ApiResult<object>.SuccessResult("角色分配成功"));
        }

        return BadRequest(ApiResult<object>.FailResult("角色分配失败"));
    }

    [HttpPost("unassign")]
    public async Task<IActionResult> UnassignRoles([FromBody] UnassignRoleRequest request)
    {
        if (request.UserId <= 0 || !request.RoleIds.Any())
        {
            return BadRequest(ApiResult<object>.FailResult("用户ID和角色ID不能为空"));
        }

        var success = await _userRoleRepository.UnassignRolesFromUserAsync(
            request.UserId,
            request.RoleIds);

        if (success)
        {
            _logger.Information("成功为用户 {UserId} 取消角色", request.UserId);
            return Ok(ApiResult<object>.SuccessResult("角色取消成功"));
        }

        return BadRequest(ApiResult<object>.FailResult("角色取消失败"));
    }

    [HttpPost("replace")]
    public async Task<IActionResult> ReplaceUserRoles([FromBody] ReplaceUserRolesRequest request)
    {
        if (request.UserId <= 0)
        {
            return BadRequest(ApiResult<object>.FailResult("用户ID不能为空"));
        }

        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Unauthorized(ApiResult<object>.FailResult("无效的用户信息"));
        }

        var success = await _userRoleRepository.ReplaceUserRolesAsync(
            request.UserId,
            request.RoleIds,
            currentUserId.Value);

        if (success)
        {
            _logger.Information("用户 {CurrentUserId} 成功替换用户 {UserId} 的角色", currentUserId, request.UserId);
            return Ok(ApiResult<object>.SuccessResult("角色替换成功"));
        }

        return BadRequest(ApiResult<object>.FailResult("角色替换失败"));
    }

    [HttpGet("user/{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(ApiResult<UserRoleResponse>.FailResult("用户ID不能为空"));
        }

        var result = await _userRoleRepository.GetUserRolesAsync(userId);
        if (result == null)
        {
            return NotFound(ApiResult<UserRoleResponse>.FailResult("用户不存在"));
        }

        return Ok(ApiResult<UserRoleResponse>.SuccessResult(result, "获取用户角色成功"));
    }

    [HttpGet("role/{roleId}/users")]
    public async Task<IActionResult> GetRoleUsers(int roleId)
    {
        if (roleId <= 0)
        {
            return BadRequest(ApiResult<RoleUserResponse>.FailResult("角色ID不能为空"));
        }

        var result = await _userRoleRepository.GetRoleUsersAsync(roleId);
        if (result == null)
        {
            return NotFound(ApiResult<RoleUserResponse>.FailResult("角色不存在"));
        }

        return Ok(ApiResult<RoleUserResponse>.SuccessResult(result, "获取角色用户成功"));
    }

    [HttpGet("check/{userId}/{roleId}")]
    public async Task<IActionResult> CheckUserRole(int userId, int roleId)
    {
        if (userId <= 0 || roleId <= 0)
        {
            return BadRequest(ApiResult<bool>.FailResult("用户ID和角色ID不能为空"));
        }

        var hasRole = await _userRoleRepository.UserHasRoleAsync(userId, roleId);
        var message = hasRole ? "用户拥有该角色" : "用户没有该角色";

        return Ok(ApiResult<bool>.SuccessResult(hasRole, message));
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetUserRoleDetails([FromQuery] int? userId = null, [FromQuery] int? roleId = null)
    {
        var result = await _userRoleRepository.GetUserRoleDetailsAsync(userId, roleId);

        var message = result.Any() ? "获取用户角色详情成功" : "没有找到相关数据";
        return Ok(ApiResult<List<UserRoleDetailResponse>>.SuccessResult(result, message));
    }
}