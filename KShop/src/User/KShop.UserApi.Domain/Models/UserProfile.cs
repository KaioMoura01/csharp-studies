namespace KShop.UserApi.Domain.Models;

public class UserProfile
{
    public Guid Id { get; set; }
    public string? KeycloakSubjectId { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
