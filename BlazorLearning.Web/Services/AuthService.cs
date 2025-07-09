using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.ApiServices;
using BlazorLearning.Shared.Services;

namespace BlazorLearning.Web.Services;

public class AuthService
{
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;

    public AuthService(
        IAuthApi authApi,
        ITokenService tokenService,
        ILoggerService logger)
    {
        _authApi = authApi;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<(bool success, string message)> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest { Username = username, Password = password };
            var response = await _authApi.LoginAsync(request);

            if (response.Success && response.Data != null)
            {
                // 使用TokenService设置登录状态
                await _tokenService.SetLoginStateAsync(response.Data);

                _logger.Information("用户 {Username} 登录成功", username);
                return (true, "登录成功");
            }
            else
            {
                _logger.Warning("登录失败，用户名: {Username}, 错误: {Error}", username, response.Message);
                return (false, response.Message ?? "登录失败");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "登录请求网络错误，用户名: {Username}", username);
            return (false, $"网络连接错误：{ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "登录过程发生异常，用户名: {Username}", username);
            return (false, $"登录异常：{ex.Message}");
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    public async Task<bool> LogoutAsync()
    {
        try
        {
            await _tokenService.ClearLoginStateAsync();
            _logger.Information("用户登出成功");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "登出过程发生异常");
            return false;
        }
    }

    /// <summary>
    /// 获取当前Token
    /// </summary>
    public async Task<string?> GetCurrentTokenAsync() => await _tokenService.GetTokenAsync();

    /// <summary>
    /// 检查是否已认证
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync() => await _tokenService.IsAuthenticatedAsync();

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public async Task<UserState?> GetCurrentUserAsync() => await _tokenService.GetUserStateAsync();

    /// <summary>
    /// 获取用户名
    /// </summary>
    public async Task<string?> GetUsernameAsync() => await _tokenService.GetUsernameAsync();

    /// <summary>
    /// 获取用户资料
    /// </summary>
    public async Task<(bool success, string message, UserDto? user)> GetUserProfileAsync()
    {
        try
        {
            var response = await _authApi.GetProfileAsync();

            if (response.Success && response.Data != null)
            {
                return (true, "获取用户资料成功", response.Data);
            }
            else
            {
                return (false, response.Message ?? "获取用户资料失败", null);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.Error(ex, "获取用户资料网络错误");
            return (false, $"网络连接错误：{ex.Message}", null);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户资料时发生异常");
            return (false, $"获取用户资料异常：{ex.Message}", null);
        }
    }
}