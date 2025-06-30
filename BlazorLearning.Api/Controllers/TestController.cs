using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorLearning.Api.Controllers;

[Route("api/[controller]")]

public class TestController : BaseApiController
{
    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return ApiOk("这是公开接口，无需认证");
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult ProtectedEndpoint()
    {
        var uerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var userInfo = new
        {
            Message = "这是受保护的接口，需要JWT认证",
            UserId = uerId,
            UserName = userName,
            Email = email,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        };

        return ApiOk(userInfo);
    }

    [HttpGet("user-info")]
    [Authorize]
    public IActionResult GetCurrentUserInfo()
    {
        var userId = User.FindFirst("userId")?.Value;
        var userName = User.FindFirst("username")?.Value;

        return ApiOk(new
        {
            Message = "当前登录用户信息",
            UserId = userId,
            UserName = userName,
            IsAuthenticated = User.Identity?.IsAuthenticated,
            AuthenticationType = User.Identity?.AuthenticationType,
        });
    }
}