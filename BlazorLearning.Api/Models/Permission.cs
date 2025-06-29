using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearning.Api.Models;

[Table(Name = "permissions")]
public class Permission
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column(StringLength = 100)]
    [Required(ErrorMessage = "权限名称不能为空")]
    public string Name { get; set; } = string.Empty;

    [Column(StringLength = 200)]
    [Required(ErrorMessage = "显示名称不能为空")]
    public string DisplayName { get; set; } = string.Empty;

    [Column(StringLength = 500)]
    public string? Description { get; set; }

    [Column(StringLength = 50)]
    public string? Category { get; set; }

    [Column]
    public bool IsActive { get; set; } = true;

    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column]
    public DateTime? UpdatedAt { get; set; }
}