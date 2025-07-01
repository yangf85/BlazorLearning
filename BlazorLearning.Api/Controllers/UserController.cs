using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
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
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<User>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResponse<IEnumerable<User>>>> GetUsers()
    {
        _logger.Information("开始获取用户列表");

        var users = await _userRepository.GetAllUsersAsync();

        _logger.Information("成功获取到 {Count} 个用户", users.Count());

        return Ok(ApiResponse<IEnumerable<User>>.SuccessResult(users, "获取用户列表成功"));
    }

    /// <summary>
    /// 根据ID获取指定用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>返回指定用户的详细信息</returns>
    /// <response code="200">成功返回用户信息</response>
    /// <response code="404">用户不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<User>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [Produces("application/json")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<User>.FailResult($"用户Id{id}不存在"));
        }
        return Ok(ApiResponse<User>.SuccessResult(user, "获取用户信息成功"));
    }

    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns>返回创建的用户信息</returns>
    /// <response code="201">用户创建成功</response>
    /// <response code="400">请求数据验证失败</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<User>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<ApiResponse<User>>> PostUser(User user) // 修改返回类型
    {
        _logger.Information("开始创建用户, Username: {Username}", user?.Username ?? "未知");

        if (!ModelState.IsValid)
        {
            _logger.Warning("创建用户时模型验证失败, Username: {Username}", user?.Username ?? "未知");
            return BadRequest(ApiResponse<User>.FailResult("模型验证失败"));
        }

        try
        {
            var createdUser = await _userRepository.CreateUserAsync(user);
            _logger.Information("用户创建成功, ID: {UserId}, Username: {Username}",
                createdUser.Id, createdUser.Username);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id },
                ApiResponse<User>.SuccessResult(createdUser, "用户创建成功"));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "创建用户失败, Username: {Username}", user?.Username ?? "未知");
            return BadRequest(ApiResponse<User>.FailResult($"创建用户失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="user">更新的用户信息</param>
    /// <returns>返回更新后的用户信息</returns>
    /// <response code="200">用户更新成功</response>
    /// <response code="400">请求数据验证失败或ID不匹配</response>
    /// <response code="404">用户不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<User>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<ActionResult<User>> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("用户ID不匹配");
        }

        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"用户Id{id}不存在");
        }

        try
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest($"更新用户失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    /// <param name="id">要删除的用户ID</param>
    /// <returns>删除操作的结果</returns>
    /// <response code="200">用户删除成功</response>
    /// <response code="404">用户不存在</response>
    /// <response code="500">服务器内部错误</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    [Produces("application/json")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"用户Id{id}不存在");
        }
        try
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);
            if (isDeleted)
            {
                return NoContent();
            }
            else
            {
                return BadRequest("删除用户失败");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"删除用户失败: {ex.Message}");
        }
    }

    [HttpGet("test-exception")]
    public ActionResult TestException()
    {
        _logger.Debug("测试异常处理");
        throw new InvalidOperationException("这是一个测试异常");
    }
}