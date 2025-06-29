using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace BlazorLearning.Api.Models;

[Table(Name = "users")]
public class User
{
    [Column(IsIdentity = true, IsPrimary = true)]
    public int Id { get; set; }

    [Column(StringLength = 50)]  // 改为50，和验证保持一致
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
}