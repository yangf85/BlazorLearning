using BlazorLearning.Api.Models;
using FreeSql;

namespace BlazorLearning.Api.Repositories;

/// <summary>
/// 角色仓储接口 - 基于FreeSql的Repository模式
/// </summary>
public interface IRoleRepository : IBaseRepository<Role>
{
    /// <summary>
    /// 根据角色名称获取角色
    /// </summary>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    /// 检查角色名是否已存在
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

    /// <summary>
    /// 获取所有激活的角色
    /// </summary>
    Task<List<Role>> GetActiveRolesAsync();

    /// <summary>
    /// 软删除角色
    /// </summary>
    Task<bool> SoftDeleteAsync(int id);
}

/// <summary>
/// 角色仓储实现类 - 继承FreeSql的BaseRepository
/// </summary>
public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    // 使用FreeSql BaseRepository的标准构造函数
    public RoleRepository(IFreeSql fsql) : base(fsql)
    {
        // 可以在这里设置一些仓储级别的配置
    }

    /// <summary>
    /// 根据角色名称获取角色
    /// </summary>
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await Select.Where(r => r.Name == name && r.IsActive).FirstAsync();
    }

    /// <summary>
    /// 检查角色名是否已存在
    /// </summary>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = Select.Where(r => r.Name == name && r.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// 获取所有激活的角色
    /// </summary>
    public async Task<List<Role>> GetActiveRolesAsync()
    {
        return await Select.Where(r => r.IsActive)
                          .OrderBy(r => r.DisplayName)
                          .ToListAsync();
    }

    /// <summary>
    /// 软删除角色
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var affectedRows = await UpdateDiy
            .Set(r => r.IsActive, false)
            .Set(r => r.UpdatedAt, DateTime.UtcNow)
            .Where(r => r.Id == id && r.IsActive)
            .ExecuteAffrowsAsync();

        return affectedRows > 0;
    }

    /// <summary>
    /// 重写插入方法，自动设置时间戳
    /// </summary>
    public override async Task<Role> InsertAsync(Role entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        return await base.InsertAsync(entity, cancellationToken);
    }

    /// <summary>
    /// 重写更新方法，自动设置更新时间
    /// </summary>
    public override async Task<int> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return await base.UpdateAsync(entity, cancellationToken);
    }
}