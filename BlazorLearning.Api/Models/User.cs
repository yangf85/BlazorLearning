using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorLearning.Api.Models;

[Table(Name = "users")]
[Index("idx_users_username", "Username", true)] // 用户名唯一索引
[Index("idx_users_email", "Email", true)]       // 邮箱唯一索引
public class User
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column(StringLength = 50)]
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "用户名长度必须在2到50个字符之间")]
    public string Username { get; set; } = string.Empty;

    [Column(StringLength = 255)]
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;

    [Column(StringLength = 60)]  // BCrypt 哈希固定长度为 60
    public string PasswordHash { get; set; } = string.Empty;

    [Column(StringLength = 100)]
    public string? FullName { get; set; }

    [Column]
    public bool IsActive { get; set; } = true;

    [Column]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column]
    public DateTime? UpdatedAt { get; set; }

    //public virtual ICollection<Role> Roles { get; set; } = [];

    //public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}