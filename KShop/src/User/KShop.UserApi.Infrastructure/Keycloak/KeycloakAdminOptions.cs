namespace KShop.UserApi.Infrastructure.Keycloak;

public class KeycloakAdminOptions
{
    public required string AdminUrl { get; init; }
    public required string Realm { get; init; }
    public required string AdminUsername { get; init; }
    public required string AdminPassword { get; init; }
}
