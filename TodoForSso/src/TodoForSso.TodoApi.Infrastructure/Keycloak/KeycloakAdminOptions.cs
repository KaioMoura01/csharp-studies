namespace TodoForSso.TodoApi.Infrastructure.Keycloak;

public class KeycloakAdminOptions
{
    public required string Realm { get; set; }
    public required string AdminUrl { get; set; }
    public required string AdminUsername { get; set; }
    public required string AdminPassword { get; set; }
    public string DefaultRole { get; set; } = "user";
}
