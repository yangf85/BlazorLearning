// BlazorLearning.Web/Services/CustomAuthStateProvider.cs
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Web.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private UserState _userState = new();
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(ILogger<CustomAuthStateProvider> logger)
    {
        _logger = logger;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_userState.IsAuthenticated && !string.IsNullOrEmpty(_userState.Token))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _userState.UserId.ToString()),
                new(ClaimTypes.Name, _userState.Username),
                new(ClaimTypes.Email, _userState.Email ?? "")
            };

            // 添加角色Claims
            foreach (var role in _userState.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticationState(user));
        }

        return Task.FromResult(new AuthenticationState(_anonymous));
    }

    /// <summary>
    /// 标记用户为已认证
    /// </summary>
    public async Task MarkUserAsAuthenticated(LoginResponse loginResponse)
    {
        _userState = new UserState
        {
            IsAuthenticated = true,
            UserId = loginResponse.UserId,
            Username = loginResponse.Username,
            Email = loginResponse.Email,
            Token = loginResponse.Token,
            Roles = loginResponse.Roles ?? new List<string>(),
            LoginTime = DateTime.Now
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _userState.UserId.ToString()),
            new(ClaimTypes.Name, _userState.Username),
            new(ClaimTypes.Email, _userState.Email ?? "")
        };

        foreach (var role in _userState.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation("用户认证状态已更新: {Username}", _userState.Username);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    /// <summary>
    /// 标记用户为已登出
    /// </summary>
    public async Task MarkUserAsLoggedOut()
    {
        var previousUsername = _userState.Username;
        _userState = new UserState();

        _logger.LogInformation("用户已登出: {Username}", previousUsername);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    /// <summary>
    /// 获取当前Token
    /// </summary>
    public string? GetCurrentToken() => _userState.Token;

    /// <summary>
    /// 获取当前用户状态
    /// </summary>
    public UserState GetUserState() => _userState;

    /// <summary>
    /// 检查是否已认证
    /// </summary>
    public bool IsAuthenticated => _userState.IsAuthenticated && !string.IsNullOrEmpty(_userState.Token);
}