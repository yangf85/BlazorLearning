using FreeSql.DataAnnotations;

namespace BlazorLearning.Api.Models;

[Table(Name = "role_permissions")]
public class RolePermission
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column]
    public int RoleId { get; set; }

    [Column]
    public int PermissionId { get; set; }

    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性
    [Navigate(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;

    [Navigate(nameof(PermissionId))]
    public virtual Permission Permission { get; set; } = null!;
}