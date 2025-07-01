using FreeSql.DataAnnotations;

namespace BlazorLearning.Api.Models;

[Table(Name = "user_roles")]
public class UserRole
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column]
    public int UserId { get; set; }

    [Column]
    public int RoleId { get; set; }

    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //[Navigate(nameof(UserId))]
    //public virtual User User { get; set; } = null!;

    //[Navigate(nameof(RoleId))]
    //public virtual Role Role { get; set; } = null!;
}