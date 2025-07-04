﻿using BlazorLearning.Api.Models;
using BlazorLearning.Shared.Services;
using FreeSql;

namespace BlazorLearning.Api.Repositories;

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

public class UserRepository : IUserRepository
{
    private readonly IFreeSql _freeSql;

    public UserRepository(IFreeSql freeSql)
    {
        _freeSql = freeSql;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _freeSql.Select<User>()
            .Where(u => u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _freeSql.Select<User>()
            .Where(u => u.Id == id && u.IsActive)
            .FirstAsync();
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;

        var result = await _freeSql.Insert(user)
            .ExecuteIdentityAsync();

        user.Id = (int)result;
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;

        await _freeSql.Update<User>()
            .SetSource(user)
            .ExecuteAffrowsAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        // 软删除：将IsActive设为false
        var affectedRows = await _freeSql.Update<User>()
            .Set(u => u.IsActive, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow)
            .Where(u => u.Id == id)
            .ExecuteAffrowsAsync();

        return affectedRows > 0;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _freeSql.Select<User>()
            .Where(u => u.Username == username && u.IsActive)
            .FirstAsync();
        return user;
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _freeSql.Select<User>()
            .Where(u => u.Email == email && u.IsActive)
            .FirstAsync();
        return user;
    }
}