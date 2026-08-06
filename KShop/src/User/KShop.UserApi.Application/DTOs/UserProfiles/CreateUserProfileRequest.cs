namespace KShop.UserApi.Application.DTOs.UserProfiles;

public record CreateUserProfileRequest(
    string KeycloakSubjectId,
    string? DisplayName,
    string? Email);
