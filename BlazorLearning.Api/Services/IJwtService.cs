using BlazorLearning.Api.Models;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Api.Services
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user); // 改为异步方法

        DateTime GetTokenExpiry();
    }
}