// BlazorLearning.Web/Services/TokenService.cs
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Web.Services;

public interface ITokenService
{
    Task SetTokenAsync(string token);

    Task<string?> GetTokenAsync();

    Task RemoveTokenAsync();

    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetUsernameAsync();

    // 新增方法：支持登录状态管理
    Task SetLoginStateAsync(LoginResponse loginResponse);

    Task ClearLoginStateAsync();

    Task<UserState?> GetUserStateAsync();
}

public class TokenService : ITokenService
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly ILogger<TokenService> _logger;
    private const string TokenKey = "authToken";

    public TokenService(
        ProtectedSessionStorage sessionStorage,
        AuthenticationStateProvider authStateProvider,
        ILogger<TokenService> logger)
    {
        _sessionStorage = sessionStorage;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        _logger = logger;
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            await _sessionStorage.SetAsync(TokenKey, token);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
        {
            // 预渲染期间忽略
            _logger.LogDebug("预渲染期间无法存储Token，已忽略");
        }
    }

    /// <summary>
    /// 设置登录状态
    /// </summary>
    public async Task SetLoginStateAsync(LoginResponse loginResponse)
    {
        // 更新AuthStateProvider状态（立即生效）
        await _authStateProvider.MarkUserAsAuthenticated(loginResponse);

        // 存储到SessionStorage（如果可用）
        await SetTokenAsync(loginResponse.Token);

        _logger.LogInformation("登录状态已设置，用户: {Username}", loginResponse.Username);
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            // 优先从AuthStateProvider获取
            var token = _authStateProvider.GetCurrentToken();
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            // 尝试从SessionStorage获取
            var result = await _sessionStorage.GetAsync<string>(TokenKey);
            return result.Success ? result.Value : null;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
        {
            // 预渲染期间从AuthStateProvider获取
            return _authStateProvider.GetCurrentToken();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "获取Token时发生错误");
            return _authStateProvider.GetCurrentToken();
        }
    }

    public async Task RemoveTokenAsync()
    {
        try
        {
            await _sessionStorage.DeleteAsync(TokenKey);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
        {
            // 预渲染期间忽略
            _logger.LogDebug("预渲染期间无法清除Token，已忽略");
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        // 优先检查AuthStateProvider的状态
        if (_authStateProvider.IsAuthenticated)
        {
            return true;
        }

        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetUsernameAsync()
    {
        // 优先从AuthStateProvider获取
        var userState = _authStateProvider.GetUserState();
        if (userState.IsAuthenticated && !string.IsNullOrEmpty(userState.Username))
        {
            return userState.Username;
        }

        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 清除登录状态
    /// </summary>
    public async Task ClearLoginStateAsync()
    {
        // 清除AuthStateProvider状态
        await _authStateProvider.MarkUserAsLoggedOut();

        // 清除SessionStorage
        await RemoveTokenAsync();

        _logger.LogInformation("登录状态已清除");
    }

    /// <summary>
    /// 获取用户状态
    /// </summary>
    public async Task<UserState?> GetUserStateAsync()
    {
        var userState = _authStateProvider.GetUserState();
        return userState.IsAuthenticated ? userState : null;
    }
}