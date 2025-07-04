namespace BlazorLearning.Shared.Dtos
{
    /// <summary>
    /// 权限检查请求
    /// </summary>
    public class PermissionCheckRequest
    {
        /// <summary>
        /// 用户ID（可选，不传则检查当前登录用户）
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 要检查的角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
    }
}