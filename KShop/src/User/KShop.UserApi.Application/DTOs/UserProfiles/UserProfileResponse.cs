namespace KShop.UserApi.Application.DTOs.UserProfiles;

public record UserProfileResponse(
    Guid Id,
    string? KeycloakSubjectId,
    string? DisplayName,
    string? Email,
    bool Active);
