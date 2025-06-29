// 1. 首先安装 BCrypt.Net-Next 包
// 在项目目录运行: dotnet add package BCrypt.Net-Next

using BCrypt.Net;

namespace BlazorLearning.Api.Utils;

/// <summary>
/// 密码加密工具类（使用 BCrypt）
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// 对密码进行哈希加密
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <returns>BCrypt 哈希值（已包含盐值）</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("密码不能为空", nameof(password));

        // BCrypt 自动生成盐值并进行哈希，工作因子为 12（推荐值）
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    /// <summary>
    /// 验证密码是否正确
    /// </summary>
    /// <param name="password">用户输入的明文密码</param>
    /// <param name="hashedPassword">数据库中存储的 BCrypt 哈希</param>
    /// <returns>密码是否匹配</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}