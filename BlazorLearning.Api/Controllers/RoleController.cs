using BlazorLearning.Api.Models;
using BlazorLearning.Api.Repositories;
using BlazorLearning.Shared.Models;
using BlazorLearning.Shared.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[Route("api/[controller]")]
[Authorize] // 所有角色管理操作都需要认证
public class RoleController : BaseApiController
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILoggerService _logger;

    public RoleController(IRoleRepository roleRepository, ILoggerService logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            _logger.Information("开始获取角色列表");

            var roles = await _roleRepository.GetActiveRolesAsync();
            var roleDtos = roles.Adapt<List<RoleDto>>();

            _logger.Information($"成功获取角色列表, 角色数量: {roles.Count}");
            return ApiOk(roleDtos, $"成功获取 {roles.Count} 个角色");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "获取角色列表失败");
            return ApiBadRequest("获取角色列表失败");
        }
    }

    /// <summary>
    /// 根据ID获取角色信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(int id)
    {
        try
        {
            _logger.Information($"开始获取角色信息, ID: {id}");

            var role = await _roleRepository.Select
                .Where(r => r.Id == id && r.IsActive)
                .FirstAsync();

            if (role == null)
            {
                _logger.Warning($"角色不存在, ID: {id}");
                return ApiNotFound("角色不存在");
            }

            var roleDto = role.Adapt<RoleDto>();
            _logger.Information($"成功获取角色信息, 角色名: {role.Name}");
            return ApiOk(roleDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"获取角色信息失败, ID: {id}");
            return ApiBadRequest("获取角色信息失败");
        }
    }

    /// <summary>
    /// 创建新角色
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            _logger.Information($"开始创建角色, 角色名: {request.Name}");

            // 检查角色名是否已存在
            var exists = await _roleRepository.ExistsByNameAsync(request.Name);
            if (exists)
            {
                _logger.Warning($"角色名已存在, 角色名: {request.Name}");
                return ApiBadRequest("角色名已存在");
            }

            // 使用 Mapster 转换
            var role = request.Adapt<Role>();

            // 保存到数据库
            await _roleRepository.InsertAsync(role);

            var roleDto = role.Adapt<RoleDto>();
            _logger.Information($"成功创建角色, ID: {role.Id}, 角色名: {role.Name}");
            return ApiOk(roleDto, "角色创建成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"创建角色失败, 角色名: {request.Name}");
            return ApiBadRequest("创建角色失败");
        }
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            _logger.Information($"开始更新角色, ID: {id}");

            // 检查角色是否存在
            var role = await _roleRepository.Select
                .Where(r => r.Id == id && r.IsActive)
                .FirstAsync();

            if (role == null)
            {
                _logger.Warning($"角色不存在, ID: {id}");
                return ApiNotFound("角色不存在");
            }

            // 检查角色名是否与其他角色重复
            var exists = await _roleRepository.ExistsByNameAsync(request.Name, id);
            if (exists)
            {
                _logger.Warning($"角色名已存在, 角色名: {request.Name}");
                return ApiBadRequest("角色名已存在");
            }

            // 使用 Mapster 更新现有实体
            request.Adapt(role);
            await _roleRepository.UpdateAsync(role);

            var roleDto = role.Adapt<RoleDto>();
            _logger.Information($"成功更新角色, ID: {role.Id}, 角色名: {role.Name}");
            return ApiOk(roleDto, "角色更新成功");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"更新角色失败, ID: {id}");
            return ApiBadRequest("更新角色失败");
        }
    }

    /// <summary>
    /// 删除角色（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        try
        {
            _logger.Information($"开始删除角色, ID: {id}");

            // 检查角色是否存在
            var role = await _roleRepository.Select
                .Where(r => r.Id == id && r.IsActive)
                .FirstAsync();

            if (role == null)
            {
                _logger.Warning($"角色不存在, ID: {id}");
                return ApiNotFound("角色不存在");
            }

            // 执行软删除
            var success = await _roleRepository.SoftDeleteAsync(id);

            if (success)
            {
                _logger.Information($"成功删除角色, ID: {id}, 角色名: {role.Name}");
                return ApiOk("角色删除成功");
            }
            else
            {
                _logger.Warning($"删除角色失败, ID: {id}");
                return ApiBadRequest("删除角色失败");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"删除角色失败, ID: {id}");
            return ApiBadRequest("删除角色失败");
        }
    }
}