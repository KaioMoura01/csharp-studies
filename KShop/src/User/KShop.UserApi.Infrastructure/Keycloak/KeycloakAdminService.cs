using System.Net.Http.Json;
using System.Text.Json.Serialization;
using KShop.UserApi.Application.Abstractions;

namespace KShop.UserApi.Infrastructure.Keycloak;

// Talks to Keycloak's Admin REST API server-to-server (no browser CORS involved) using the
// realm admin credentials, since the "kshop-frontend" public client cannot create users
// itself. This keeps the admin credentials out of the SPA.
public class KeycloakAdminService(HttpClient httpClient, KeycloakAdminOptions options) : IKeycloakAdminService
{
    public async Task<string> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken cancellationToken)
    {
        var adminToken = await GetAdminTokenAsync(cancellationToken);

        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post, $"{options.AdminUrl}/admin/realms/{options.Realm}/users")
        {
            Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken) },
            Content = JsonContent.Create(new
            {
                username = request.Username,
                email = request.Email,
                firstName = request.FirstName,
                lastName = request.LastName,
                enabled = true,
                emailVerified = true,
                credentials = new[] { new { type = "password", value = request.Password, temporary = false } },
            }),
        };

        using var createResponse = await httpClient.SendAsync(createRequest, cancellationToken);
        createResponse.EnsureSuccessStatusCode();

        var location = createResponse.Headers.Location!.ToString();
        var userId = location[(location.LastIndexOf('/') + 1)..];

        await AssignRealmRoleAsync(userId, "customer", adminToken, cancellationToken);

        return userId;
    }

    private async Task<string> GetAdminTokenAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"{options.AdminUrl}/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = options.AdminUsername,
                ["password"] = options.AdminPassword,
            }),
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
        return token!.AccessToken;
    }

    private async Task AssignRealmRoleAsync(string userId, string roleName, string adminToken, CancellationToken cancellationToken)
    {
        using var roleRequest = new HttpRequestMessage(
            HttpMethod.Get, $"{options.AdminUrl}/admin/realms/{options.Realm}/roles/{roleName}")
        {
            Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken) },
        };
        using var roleResponse = await httpClient.SendAsync(roleRequest, cancellationToken);
        roleResponse.EnsureSuccessStatusCode();
        var role = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>(cancellationToken);

        using var assignRequest = new HttpRequestMessage(
            HttpMethod.Post, $"{options.AdminUrl}/admin/realms/{options.Realm}/users/{userId}/role-mappings/realm")
        {
            Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken) },
            Content = JsonContent.Create(new[] { role }),
        };
        using var assignResponse = await httpClient.SendAsync(assignRequest, cancellationToken);
        assignResponse.EnsureSuccessStatusCode();
    }

    private record KeycloakTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);

    private record KeycloakRole(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name);
}
