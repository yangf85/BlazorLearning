using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Dtos;
using BlazorLearning.Shared.Services;
using FreeSql;

namespace BlazorLearning.Api.Repositories;

public interface IUserRoleRepository
{
    Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds, int assignedBy);

    Task<bool> UnassignRolesFromUserAsync(int userId, List<int> roleIds);

    Task<bool> ReplaceUserRolesAsync(int userId, List<int> roleIds, int assignedBy);

    Task<UserRoleResponse> GetUserRolesAsync(int userId);

    Task<RoleUserResponse> GetRoleUsersAsync(int roleId);

    Task<bool> UserHasRoleAsync(int userId, int roleId);

    Task<List<UserRoleDetailResponse>> GetUserRoleDetailsAsync(int? userId = null, int? roleId = null);

    Task<UserRole> GetByUserAndRoleAsync(int userId, int roleId);
}

public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
{
    private readonly ILoggerService _logger;

    public UserRoleRepository(IFreeSql freeSql, ILoggerService logger) : base(freeSql)
    {
        _logger = logger;
    }

    public async Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds, int assignedBy)
    {
        try
        {
            var existingRoles = await Select
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId) && ur.IsActive)
                .ToListAsync(ur => ur.RoleId);

            var newRoleIds = roleIds.Except(existingRoles).ToList();

            if (!newRoleIds.Any())
            {
                _logger.Information("用户 {UserId} 已拥有所有指定角色", userId);
                return true;
            }

            var userRoles = newRoleIds.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await InsertAsync(userRoles);
            _logger.Information("成功为用户 {UserId} 分配 {Count} 个新角色", userId, newRoleIds.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "为用户 {UserId} 分配角色失败", userId);
            return false;
        }
    }

    public async Task<bool> UnassignRolesFromUserAsync(int userId, List<int> roleIds)
    {
        try
        {
            var affectedRows = await UpdateDiy
                .Set(ur => ur.IsActive, false)
                .Set(ur => ur.UpdatedAt, DateTime.UtcNow)
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId) && ur.IsActive)
                .ExecuteAffrowsAsync();

            _logger.Information("成功为用户 {UserId} 取消 {Count} 个角色", userId, affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "为用户 {UserId} 取消角色失败", userId);
            return false;
        }
    }

    public async Task<bool> ReplaceUserRolesAsync(int userId, List<int> roleIds, int assignedBy)
    {
        using var transaction = Orm.Ado.TransactionCurrentThread;
        try
        {
            await UpdateDiy
                .Set(ur => ur.IsActive, false)
                .Set(ur => ur.UpdatedAt, DateTime.UtcNow)
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .ExecuteAffrowsAsync();

            if (roleIds.Any())
            {
                var userRoles = roleIds.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedBy = assignedBy,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                await InsertAsync(userRoles);
            }

            transaction?.Commit();
            _logger.Information("成功替换用户 {UserId} 的角色", userId);
            return true;
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            _logger.Error(ex, "替换用户 {UserId} 角色失败", userId);
            return false;
        }
    }

    public async Task<UserRoleResponse> GetUserRolesAsync(int userId)
    {
        try
        {
            var userWithRoles = await Select
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive && ur.Role.IsActive)
                .ToListAsync();

            if (!userWithRoles.Any())
            {
                var user = await Orm.Select<User>().Where(u => u.Id == userId).FirstAsync();
                if (user == null) return null;

                return new UserRoleResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = new List<RoleInfo>()
                };
            }

            var firstUser = userWithRoles.First().User;
            return new UserRoleResponse
            {
                UserId = firstUser.Id,
                Username = firstUser.Username,
                Email = firstUser.Email,
                Roles = userWithRoles.Select(ur => new RoleInfo
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description,
                    AssignedAt = ur.AssignedAt,
                    IsActive = ur.IsActive
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户 {UserId} 角色列表失败", userId);
            return null;
        }
    }

    public async Task<RoleUserResponse> GetRoleUsersAsync(int roleId)
    {
        try
        {
            var roleWithUsers = await Select
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.RoleId == roleId && ur.IsActive && ur.User.IsActive)
                .ToListAsync();

            if (!roleWithUsers.Any())
            {
                var role = await Orm.Select<Role>().Where(r => r.Id == roleId).FirstAsync();
                if (role == null) return null;

                return new RoleUserResponse
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description,
                    Users = new List<UserInfo>()
                };
            }

            var firstRole = roleWithUsers.First().Role;
            return new RoleUserResponse
            {
                RoleId = firstRole.Id,
                RoleName = firstRole.Name,
                Description = firstRole.Description,
                Users = roleWithUsers.Select(ur => new UserInfo
                {
                    Id = ur.User.Id,
                    Username = ur.User.Username,
                    Email = ur.User.Email,
                    AssignedAt = ur.AssignedAt,
                    IsActive = ur.IsActive
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取角色 {RoleId} 用户列表失败", roleId);
            return null;
        }
    }

    public async Task<bool> UserHasRoleAsync(int userId, int roleId)
    {
        try
        {
            return await Select
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "检查用户 {UserId} 是否拥有角色 {RoleId} 失败", userId, roleId);
            return false;
        }
    }

    public async Task<List<UserRoleDetailResponse>> GetUserRoleDetailsAsync(int? userId = null, int? roleId = null)
    {
        try
        {
            var query = Select
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Include(ur => ur.AssignedByUser)
                .Where(ur => ur.IsActive);

            if (userId.HasValue)
                query = query.Where(ur => ur.UserId == userId.Value);

            if (roleId.HasValue)
                query = query.Where(ur => ur.RoleId == roleId.Value);

            var userRoles = await query.ToListAsync();

            return userRoles.Select(ur => new UserRoleDetailResponse
            {
                Id = ur.Id,
                UserId = ur.UserId,
                Username = ur.User.Username,
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name,
                AssignedBy = ur.AssignedBy,
                AssignedByUsername = ur.AssignedByUser.Username,
                AssignedAt = ur.AssignedAt,
                IsActive = ur.IsActive,
                CreatedAt = ur.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户角色详情失败");
            return new List<UserRoleDetailResponse>();
        }
    }

    public async Task<UserRole> GetByUserAndRoleAsync(int userId, int roleId)
    {
        try
        {
            return await Select
                .Where(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive)
                .FirstAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取用户 {UserId} 和角色 {RoleId} 的关联记录失败", userId, roleId);
            return null;
        }
    }
}