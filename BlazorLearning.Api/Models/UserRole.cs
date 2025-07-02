using FreeSql.DataAnnotations;

namespace BlazorLearning.Api.Models
{
    [Table(Name = "user_roles")]
    [Index("idx_user_role_unique", "UserId,RoleId,IsActive", true)]
    [Index("idx_user_role_userid", "UserId", false)]
    [Index("idx_user_role_roleid", "RoleId", false)]
    public class UserRole
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        [Column(IsNullable = false)]
        public int UserId { get; set; }

        [Column(IsNullable = false)]
        public int RoleId { get; set; }

        [Column(IsNullable = false)]
        public int AssignedBy { get; set; }

        [Column(IsNullable = false)]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Column(IsNullable = false)]
        public bool IsActive { get; set; } = true;

        [Column(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(IsNullable = false)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Navigate(nameof(UserId))]
        public User User { get; set; }

        [Navigate(nameof(RoleId))]
        public Role Role { get; set; }

        [Navigate(nameof(AssignedBy))]
        public User AssignedByUser { get; set; }
    }
}