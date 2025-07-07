using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorLearning.Web.Services;

public interface ITokenService
{
    Task SetTokenAsync(string token);

    Task<string?> GetTokenAsync();

    Task RemoveTokenAsync();

    Task<bool> IsAuthenticatedAsync();

    Task<string?> GetUsernameAsync();
}

public class TokenService : ITokenService
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private const string TokenKey = "authToken";

    public TokenService(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task SetTokenAsync(string token)
    {
        await _sessionStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        var result = await _sessionStorage.GetAsync<string>(TokenKey);
        return result.Success ? result.Value : null;
    }

    public async Task RemoveTokenAsync()
    {
        await _sessionStorage.DeleteAsync(TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetUsernameAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }
        catch
        {
            return null;
        }
    }
}