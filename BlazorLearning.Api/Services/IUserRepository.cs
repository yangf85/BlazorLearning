using BlazorLearning.Api.Models;

namespace BlazorLearning.Api.Services;

public interface IUserRepository
{
    /// <summary>
    /// 获取所有用户
    /// </summary>
    Task<List<User>> GetAllUsersAsync();

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<bool> DeleteUserAsync(int id);

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByEmailAsync(string email);
}