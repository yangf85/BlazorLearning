using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorLearning.Api.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        IUserRoleRepository userRoleRepository,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 获取用户角色
        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
        var roleNames = userRoles?.Roles?.Select(r => r.Name).ToList() ?? new List<string>();

        // 创建Claims列表
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // 添加角色Claims
        foreach (var roleName in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        _logger.LogInformation("为用户 {Username} 生成JWT Token，包含角色: {Roles}",
            user.Username, string.Join(", ", roleNames));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: GetTokenExpiry(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiry()
    {
        return DateTime.UtcNow.AddHours(_jwtSettings.ExpireHours);
    }
}