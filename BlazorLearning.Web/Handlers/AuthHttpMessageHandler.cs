// BlazorLearning.Web/Handlers/AuthHttpMessageHandler.cs (清理后)
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorLearning.Web.Services;
using BlazorLearning.Shared.Services;

namespace BlazorLearning.Web.Handlers;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILoggerService _logger;

    public AuthHttpMessageHandler(
        AuthenticationStateProvider authStateProvider,
        ILoggerService logger)
    {
        _authStateProvider = authStateProvider;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // 获取当前Token
            var token = ((CustomAuthStateProvider)_authStateProvider).GetCurrentToken();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.Debug("已添加认证头到请求: {RequestUri}", request.RequestUri);
            }
            else
            {
                _logger.Debug("未找到Token，发送无认证请求: {RequestUri}", request.RequestUri);
            }
        }
        catch (Exception ex)
        {
            _logger.Warning("获取Token时发生错误，继续无认证请求: {RequestUri}", request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}