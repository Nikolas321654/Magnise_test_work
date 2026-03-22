using System.Text.Json.Serialization;

namespace Magnise.Infrastructure.Fintacharts.DTO;

public class TokenResponse
{
    [JsonPropertyName("access_token")] 
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")] 
    public int ExpiresIn { get; set; } 
}