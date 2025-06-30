using BlazorLearning.Api.Models;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);

        DateTime GetTokenExpiry();
    }
}