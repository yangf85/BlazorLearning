namespace BlazorLearning.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailResult(string message = "操作失败")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
            };
        }
    }
}