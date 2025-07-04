using Microsoft.AspNetCore.Authorization;

namespace BlazorLearning.Api.Attributes
{
    // 自定义角色要求特性
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}