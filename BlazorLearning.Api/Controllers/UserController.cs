using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggerService _logger;

    public UserController(IUserRepository userRepository, ILoggerService logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    /// <returns>返回所有活跃用户的列表</returns>
    /// <response code="200">成功返回用户列表</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<List<UserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 500)]
    [Produces("application/json")]
    public async Task<IActionResult> GetUsers()
    {
        _logger.Information("开始获取用户列表");

        try
        {
            var users = await _userRepository.GetAllUsersAsync();

            // 将User实体转换为UserDto
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            _logger.Information("成功获取到 {Count} 个用户", userDtos.Count);

            return ApiOk(userDtos, "获取用户列表成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户列表时发生错误");
            return ApiBadRequest("获取用户列表失败");
        }
    }

    /// <summary>
    /// 根据ID获取指定用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>返回指定用户的详细信息</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 500)]
    [Produces("application/json")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return ApiNotFound($"用户Id{id}不存在");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return ApiOk(userDto, "获取用户信息成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户信息时发生错误, UserId: {UserId}", id);
            return ApiBadRequest("获取用户信息失败");
        }
    }

    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns>返回创建的用户信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PostUser(User user)
    {
        _logger.Information("开始创建用户, Username: {Username}", user?.Username ?? "未知");

        if (!ModelState.IsValid)
        {
            _logger.Warning("创建用户时模型验证失败, Username: {Username}", user?.Username ?? "未知");
            return ApiBadRequest("模型验证失败");
        }

        try
        {
            var createdUser = await _userRepository.CreateUserAsync(user);

            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                IsActive = createdUser.IsActive,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            };

            _logger.Information("用户创建成功, ID: {UserId}, Username: {Username}",
                createdUser.Id, createdUser.Username);

            return ApiOk(userDto, "用户创建成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "创建用户失败, Username: {Username}", user?.Username ?? "未知");
            return ApiBadRequest($"创建用户失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="user">更新的用户信息</param>
    /// <returns>返回更新后的用户信息</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 400)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return ApiBadRequest("用户ID不匹配");
        }

        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return ApiNotFound($"用户Id{id}不存在");
        }

        try
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);

            var userDto = new UserDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                IsActive = updatedUser.IsActive,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };

            return ApiOk(userDto, "用户更新成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "更新用户失败, UserId: {UserId}", id);
            return ApiBadRequest($"更新用户失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    /// <param name="id">要删除的用户ID</param>
    /// <returns>删除操作的结果</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResult<bool>), 200)]
    [ProducesResponseType(typeof(ApiResult<object>), 404)]
    [ProducesResponseType(typeof(ApiResult<object>), 500)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return ApiNotFound($"用户Id{id}不存在");
        }

        try
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);
            if (isDeleted)
            {
                _logger.Information("成功删除用户, UserId: {UserId}, Username: {Username}",
                    id, existingUser.Username);
                return ApiOk(true, "用户删除成功");
            }
            else
            {
                _logger.Warning("删除用户失败, UserId: {UserId}", id);
                return ApiBadRequest("删除用户失败");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "删除用户失败, UserId: {UserId}", id);
            return ApiBadRequest($"删除用户失败: {ex.Message}");
        }
    }

    [HttpGet("test-exception")]
    public IActionResult TestException()
    {
        _logger.Debug("测试异常处理");
        throw new InvalidOperationException("这是一个测试异常");
    }
}