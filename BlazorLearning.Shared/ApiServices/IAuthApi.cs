using Refit;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Shared.ApiServices;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<ApiResult<string>> RegisterAsync([Body] RegisterRequest request);

    [Post("/api/auth/login")]
    Task<ApiResult<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Get("/api/auth/profile")]
    Task<ApiResult<UserDto>> GetProfileAsync();
}