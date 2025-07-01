using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorLearning.Api.Models;

[Table(Name = "roles")]
[Index("idx_roles_name", "Name", true)] // 角色名唯一索引
public class Role
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column(StringLength = 50)]
    [Required(ErrorMessage = "角色名称不能为空")]
    public string Name { get; set; } = string.Empty;

    [Column(StringLength = 100)]
    [Required(ErrorMessage = "显示名称不能为空")]
    public string DisplayName { get; set; } = string.Empty;

    [Column(StringLength = 500)]
    public string? Description { get; set; }

    [Column]
    public bool IsActive { get; set; } = true;

    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column]
    public DateTime? UpdatedAt { get; set; }

    //[Navigate(ManyToMany = typeof(UserRole))]
    //public virtual ICollection<User> Users { get; set; } = [];

    //[Navigate(ManyToMany = typeof(RolePermission))]
    //public virtual ICollection<Permission> Permissions { get; set; } = [];

    //public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    //public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
}