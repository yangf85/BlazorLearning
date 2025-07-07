using BlazorLearning.Web.Services;

namespace BlazorLearning.Web.Handlers
{
    /// <summary>
    /// 自动为HTTP请求添加JWT认证头的处理器
    /// </summary>
    public class AuthHttpMessageHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthHttpMessageHandler> _logger;

        public AuthHttpMessageHandler(ITokenService tokenService, ILogger<AuthHttpMessageHandler> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // 尝试获取Token，如果失败则静默处理
                var token = await GetTokenSafelyAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    // 添加认证头
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug("为请求 {RequestUri} 添加了认证头", request.RequestUri);
                }
                else
                {
                    _logger.LogDebug("请求 {RequestUri} 没有可用的Token", request.RequestUri);
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // 在预渲染期间，JavaScript不可用，这是正常情况
                _logger.LogDebug("预渲染期间跳过Token获取，请求: {RequestUri}", request.RequestUri);
            }
            catch (Exception ex)
            {
                // 其他错误也要静默处理，不影响请求
                _logger.LogWarning(ex, "获取Token时发生错误，继续无认证请求: {RequestUri}", request.RequestUri);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string?> GetTokenSafelyAsync()
        {
            try
            {
                return await _tokenService.GetTokenAsync();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // 预渲染期间JavaScript不可用
                return null;
            }
            catch (Exception)
            {
                // 其他错误也返回null
                return null;
            }
        }
    }
}