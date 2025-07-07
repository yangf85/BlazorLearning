using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggerService _logger;

    public TestController(IUserRepository userRepository, ILoggerService logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 公开的用户列表接口（无需认证）
    /// </summary>
    [HttpGet("users-public")]
    [ProducesResponseType(typeof(ApiResult<List<UserDto>>), 200)]
    public async Task<ActionResult<ApiResult<List<UserDto>>>> GetUsersPublic()
    {
        _logger.Information("开始获取用户列表（公开接口）");

        try
        {
            var users = await _userRepository.GetAllUsersAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            _logger.Information("成功获取到 {Count} 个用户（公开接口）", userDtos.Count);

            return Ok(ApiResult<List<UserDto>>.SuccessResult(userDtos, "获取用户列表成功"));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户列表时发生错误（公开接口）");
            return StatusCode(500, ApiResult<List<UserDto>>.FailResult("获取用户列表失败"));
        }
    }

    /// <summary>
    /// 检查原始用户接口的认证状态
    /// </summary>
    [HttpGet("check-auth")]
    public ActionResult CheckAuth()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var hasAuth = !string.IsNullOrEmpty(authHeader);

        return Ok(new
        {
            HasAuthHeader = hasAuth,
            AuthHeader = authHeader?.Substring(0, Math.Min(20, authHeader.Length)) + "...",
            Message = hasAuth ? "有认证头" : "无认证头"
        });
    }
}