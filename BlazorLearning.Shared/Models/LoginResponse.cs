// BlazorLearning.Shared/Models/LoginResponse.cs
namespace BlazorLearning.Shared.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public UserDto User { get; set; } = new();

        // 为了兼容新的认证系统，添加这些便捷属性
        public int UserId => User.Id;

        public string Username => User.Username;
        public string Email => User.Email;

        // 角色信息
        public List<string> Roles { get; set; } = new();
    }
}