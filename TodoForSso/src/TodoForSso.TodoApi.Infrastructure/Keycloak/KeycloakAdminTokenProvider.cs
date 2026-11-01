using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Keycloak;

/// <summary>
/// Busca um token de admin no realm master (bootstrap superadmin do Keycloak) para uso
/// pelo Keycloak.Net.Core, cuja API só aceita um <see cref="Func{TResult}"/> síncrono para o token.
/// </summary>
public class KeycloakAdminTokenProvider(HttpClient httpClient, IOptions<KeycloakAdminOptions> options)
{
    private readonly KeycloakAdminOptions _options = options.Value;
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public string GetToken() => GetTokenAsync().GetAwaiter().GetResult();

    private async Task<string> GetTokenAsync()
    {
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAt)
        {
            return _cachedToken;
        }

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _options.AdminUsername,
            ["password"] = _options.AdminPassword,
        };

        using var response = await httpClient.PostAsync(
            "/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(form));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new DomainException($"Falha ao autenticar no Keycloak como admin: {response.StatusCode} - {error}");
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>()
            ?? throw new DomainException("Keycloak não retornou access_token.");

        _cachedToken = token.AccessToken;
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(Math.Max(token.ExpiresIn - 10, 5));
        return _cachedToken;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
