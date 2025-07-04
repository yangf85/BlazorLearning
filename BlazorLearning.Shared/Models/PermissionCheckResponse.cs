namespace BlazorLearning.Shared.Dtos
{
    // 权限检查响应
    public class PermissionCheckResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool HasRole { get; set; } // 是否拥有该角色
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}