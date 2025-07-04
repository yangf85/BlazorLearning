using BlazorLearning.Api.Attributes;
using BlazorLearning.Api.Models;
using BlazorLearning.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers
{
    [Route("api/admin")]
    [Authorize]
    public class AdminController : BaseApiController
    {
        // 只有Admin角色才能访问
        [HttpGet("dashboard")]
        [RequireRole("Admin")]
        public ActionResult<ApiResponse<object>> GetAdminDashboard()
        {
            var data = new
            {
                Message = "欢迎进入管理员控制台",
                AccessTime = DateTime.UtcNow,
                UserInfo = new
                {
                    UserId = GetCurrentUserId(),
                    Username = GetCurrentUsername()
                }
            };

            return Ok(ApiResponse<object>.SuccessResult(data));
        }

        // 只有Admin或Manager角色才能访问
        [HttpGet("reports")]
        [RequireRole("Admin", "Manager")]
        public ActionResult<ApiResponse<object>> GetReports()
        {
            var data = new
            {
                Message = "系统报表数据",
                ReportType = "用户统计",
                GeneratedAt = DateTime.UtcNow,
                Data = new[]
                {
                    new { Date = "2025-07-01", Users = 150 },
                    new { Date = "2025-07-02", Users = 165 }
                }
            };

            return Ok(ApiResponse<object>.SuccessResult(data));
        }

        // 只有Admin角色才能访问系统设置
        [HttpGet("settings")]
        [RequireRole("Admin")]
        public ActionResult<ApiResponse<object>> GetSystemSettings()
        {
            var data = new
            {
                Message = "系统设置",
                Settings = new
                {
                    DatabaseConnection = "Active",
                    CacheEnabled = true,
                    LogLevel = "Information",
                    MaintenanceMode = false
                },
                LastModified = DateTime.UtcNow
            };

            return Ok(ApiResponse<object>.SuccessResult(data));
        }

        // 任何登录用户都可以访问
        [HttpGet("profile")]
        public ActionResult<ApiResponse<object>> GetProfile()
        {
            var data = new
            {
                Message = "个人资料页面",
                UserId = GetCurrentUserId(),
                Username = GetCurrentUsername(),
                AccessLevel = "Standard User"
            };

            return Ok(ApiResponse<object>.SuccessResult(data));
        }
    }
}