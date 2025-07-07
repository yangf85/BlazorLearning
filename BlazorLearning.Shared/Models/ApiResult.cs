namespace BlazorLearning.Shared.Models;

public class ApiResult<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public static ApiResult<T> SuccessResult(T data, string message = "操作成功")
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResult<T> FailResult(string message = "操作失败")
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
        };
    }
}