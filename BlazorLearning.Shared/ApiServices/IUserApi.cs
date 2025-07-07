using Refit;
using BlazorLearning.Shared.Models;

namespace BlazorLearning.Shared.ApiServices;

public interface IUserApi
{
    [Get("/api/users")]
    Task<ApiResult<List<UserDto>>> GetAllUsersAsync();

    [Get("/api/users/{id}")]
    Task<ApiResult<UserDto>> GetUserByIdAsync(int id);

    [Post("/api/users")]
    Task<ApiResult<UserDto>> CreateUserAsync([Body] UserDto userDto);

    [Put("/api/users/{id}")]
    Task<ApiResult<UserDto>> UpdateUserAsync(int id, [Body] UserDto userDto);

    [Delete("/api/users/{id}")]
    Task<ApiResult<bool>> DeleteUserAsync(int id);

    // 测试用的公开接口
    [Get("/api/test/users-public")]
    Task<ApiResult<List<UserDto>>> GetUsersPublicAsync();
}