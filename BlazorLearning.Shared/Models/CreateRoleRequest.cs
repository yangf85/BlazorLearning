using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorLearning.Shared.Models;

/// <summary>
/// 创建角色请求模型
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// 角色名称（英文标识）
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色名称长度必须在2到50个字符之间")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色显示名称（中文名称）
    /// </summary>
    [Required(ErrorMessage = "角色显示名称不能为空")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "角色显示名称长度必须在2到100个字符之间")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    [StringLength(500, ErrorMessage = "角色描述不能超过500个字符")]
    public string? Description { get; set; }
}