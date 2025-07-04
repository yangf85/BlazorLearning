namespace BlazorLearning.Shared.Dtos
{
    // 用户权限概览
    public class UserPermissionOverview
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new(); // 用户拥有的所有角色
        public bool IsAdmin { get; set; } // 是否为管理员
        public bool IsActive { get; set; } // 是否为活跃用户
    }
}