using BlazorLearning.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorLearning.Api.Controllers;

[ApiController]
public class BaseApiController : ControllerBase
{
    protected int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }

    protected string GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value;
    }

    protected IActionResult ApiOk<T>(T data, string message = "操作成功")
    {
        return base.Ok(ApiResult<T>.SuccessResult(data, message));
    }

    protected IActionResult ApiOk(string message = "操作成功")
    {
        return base.Ok(ApiResult<object>.SuccessResult(null, message));
    }

    protected IActionResult ApiBadRequest(string message = "请求参数错误")
    {
        return base.BadRequest(ApiResult<object>.FailResult(message));
    }

    protected IActionResult ApiUnauthorized(string message = "未经授权的请求")
    {
        return base.Unauthorized(ApiResult<object>.FailResult(message));
    }

    protected IActionResult ApiNotFound(string message = "资源未找到")
    {
        return base.NotFound(ApiResult<object>.FailResult(message));
    }
}