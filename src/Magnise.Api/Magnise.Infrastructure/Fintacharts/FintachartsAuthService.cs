using System.Text.Json;
using Magnise.Infrastructure.Fintacharts.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Magnise.Infrastructure.Fintacharts;

// Infrastructure/Fintacharts/FintachartsAuthService.cs
public class FintachartsAuthService(
    HttpClient httpClient,
    IConfiguration config,
    ILogger<FintachartsAuthService> logger)
{
    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private DateTime _expiresAt = DateTime.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
            return _accessToken;

        if (!string.IsNullOrEmpty(_refreshToken))
        {
            try
            {
                await RefreshAsync(ct);
                return _accessToken;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Refresh failed, re-authenticating...");
            }
        }

        await AuthenticateAsync(ct);
        return _accessToken;
    }

    private async Task AuthenticateAsync(CancellationToken ct)
    {
        var cfg = config.GetSection("Fintacharts");

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = cfg["ClientId"]!,
            ["username"] = cfg["Username"]!,
            ["password"] = cfg["Password"]!
        };

        var response = await SendTokenRequestAsync(cfg["Realm"]!, form, ct);
        ApplyToken(response);

        logger.LogInformation("Authenticated successfully. Token expires at {ExpiresAt}", _expiresAt);
    }

    private async Task RefreshAsync(CancellationToken ct)
    {
        var cfg = config.GetSection("Fintacharts");

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = cfg["ClientId"]!,
            ["refresh_token"] = _refreshToken
        };

        var response = await SendTokenRequestAsync(cfg["Realm"]!, form, ct);
        ApplyToken(response);

        logger.LogInformation("Token refreshed. New expiry: {ExpiresAt}", _expiresAt);
    }

    private async Task<TokenResponse> SendTokenRequestAsync(
        string realm,
        Dictionary<string, string> form,
        CancellationToken ct)
    {
        var url = $"/identity/realms/{realm}/protocol/openid-connect/token";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(form)
        };

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TokenResponse>(json)
               ?? throw new Exception("Failed to deserialize token response");
    }

    private void ApplyToken(TokenResponse token)
    {
        _accessToken = token.AccessToken;
        _refreshToken = token.RefreshToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 300);
    }
}