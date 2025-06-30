using BlazorLearning.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult ApiOk<T>(T data, string message = "操作成功")
        {
            return base.Ok(ApiResponse<T>.SuccessResult(data, message));
        }

        protected IActionResult ApiOk(string message = "操作成功")
        {
            return base.Ok(ApiResponse<object>.SuccessResult(null, message));
        }

        protected IActionResult ApiBadRequest(string message = "请求参数错误")
        {
            return base.BadRequest(ApiResponse<object>.FailResult(message));
        }

        protected IActionResult ApiUnauthorized(string message = "未经授权的请求")
        {
            return base.Unauthorized(ApiResponse<object>.FailResult(message));
        }

        protected IActionResult ApiNotFound(string message = "资源未找到")
        {
            return base.NotFound(ApiResponse<object>.FailResult(message));
        }
    }
}