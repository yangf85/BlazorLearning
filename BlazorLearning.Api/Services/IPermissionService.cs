using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Dtos;
using System.Security.Claims;

namespace BlazorLearning.Shared.Services;

// 权限服务接口
public interface IPermissionService
{
    // 检查用户是否拥有指定角色
    Task<bool> HasRoleAsync(int userId, string roleName);

    // 检查当前登录用户是否拥有指定角色
    Task<bool> CurrentUserHasRoleAsync(string roleName);

    // 获取用户的所有角色
    Task<List<string>> GetUserRolesAsync(int userId);

    // 获取用户权限概览
    Task<UserPermissionOverview?> GetUserPermissionOverviewAsync(int userId);

    // 检查用户是否为管理员
    Task<bool> IsAdminAsync(int userId);
}

// 权限服务实现
public class PermissionService : IPermissionService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PermissionService> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    // 检查用户是否拥有指定角色
    public async Task<bool> HasRoleAsync(int userId, string roleName)
    {
        try
        {
            // 先根据角色名获取角色ID
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }

            // 使用你现有的方法检查用户是否拥有该角色
            return await _userRoleRepository.UserHasRoleAsync(userId, role.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查用户 {UserId} 角色 {RoleName} 失败", userId, roleName);
            return false;
        }
    }

    // 检查当前登录用户是否拥有指定角色
    public async Task<bool> CurrentUserHasRoleAsync(string roleName)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return false;
        }

        return await HasRoleAsync(currentUserId.Value, roleName);
    }

    // 获取用户的所有角色
    public async Task<List<string>> GetUserRolesAsync(int userId)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId);
            if (userRoles == null)
            {
                return new List<string>();
            }
            return userRoles.Roles.Select(r => r.Name).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户 {UserId} 角色列表失败", userId);
            return new List<string>();
        }
    }

    // 获取用户权限概览
    public async Task<UserPermissionOverview?> GetUserPermissionOverviewAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await GetUserRolesAsync(userId);
            var isAdmin = roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

            return new UserPermissionOverview
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                IsAdmin = isAdmin,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户 {UserId} 权限概览失败", userId);
            return null;
        }
    }

    // 检查用户是否为管理员
    public async Task<bool> IsAdminAsync(int userId)
    {
        try
        {
            var roles = await GetUserRolesAsync(userId);
            var isAdmin = roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
            _logger.LogInformation("用户 {UserId} 角色检查: 拥有角色 [{Roles}], 是否管理员: {IsAdmin}",
                userId, string.Join(", ", roles), isAdmin);
            return isAdmin;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查用户 {UserId} 管理员权限失败", userId);
            return false;
        }
    }

    // 获取当前登录用户ID
    private int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }
}