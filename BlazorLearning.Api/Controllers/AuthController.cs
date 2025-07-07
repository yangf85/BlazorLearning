using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Api.Services;
using BlazorLearning.Api.Utils;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorLearning.Api.Controllers;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    IUserRepository _userRepository;
    ILoggerService _logger;

    IJwtService _jwt;

    public AuthController(IUserRepository userRepository, ILoggerService logge, IJwtService jwt)
    {
        _userRepository = userRepository;
        _logger = logge;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.Information($"开始用户注册,用户名称:{request.Username}");
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return ApiBadRequest("用户名已存在");
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var createdUser = await _userRepository.CreateUserAsync(user);
            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt,
                IsActive = createdUser.IsActive
            };
            return ApiOk(userDto, "注册成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"用户注册失败,用户名称{request.Username}");
            return ApiBadRequest("注册失败，请稍后再试");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.Information($"开始用户登录,用户名称:{request.Username}");
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive)
            {
                _logger.Warning($"登录失败，用户不存在或已被禁用,用户名称:{request.Username}");
                return ApiUnauthorized("用户名或密码错误");
            }

            if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.Warning($"登录失败，密码错误,用户名称:{request.Username}");
                return ApiUnauthorized("用户名或密码错误");
            }

            var token = await _jwt.GenerateTokenAsync(user);
            var expiry = _jwt.GetTokenExpiry();

            _logger.Information($"用户登录成功,用户名称:{request.Username}");

            var loginResponse = new LoginResponse
            {
                Token = token,
                Expiry = expiry,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsActive = user.IsActive
                },
            };

            return ApiOk(loginResponse, "登录成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"用户登录失败,用户名称{request.Username}");
            return ApiBadRequest("登录失败，请稍后再试");
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return ApiUnauthorized("无效的用户信息");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return ApiNotFound("用户不存在或已被禁用");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            };

            return ApiOk(userDto, "获取用户信息成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户信息失败");
            return ApiBadRequest("获取用户信息失败，请稍后再试");
        }
    }
}