namespace KShop.UserApi.Application.DTOs.UserProfiles;

public record UpdateUserProfileRequest(
    Guid Id,
    string? DisplayName,
    string? Email);
