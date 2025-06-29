namespace BlazorLearning.Shared.Services
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// 记录信息日志
        /// </summary>
        void Information(string message, params object[] args);

        /// <summary>
        /// 记录警告日志
        /// </summary>
        void Warning(string message, params object[] args);

        /// <summary>
        /// 记录错误日志
        /// </summary>
        void Error(string message, params object[] args);

        /// <summary>
        /// 记录错误日志（带异常）
        /// </summary>
        void Error(Exception exception, string message, params object[] args);

        /// <summary>
        /// 记录调试日志
        /// </summary>
        void Debug(string message, params object[] args);
    }
}