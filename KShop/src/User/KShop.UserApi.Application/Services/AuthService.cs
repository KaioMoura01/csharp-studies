using KShop.UserApi.Application.Abstractions;
using KShop.UserApi.Application.DTOs.Auth;
using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Application.Services.Interfaces;

namespace KShop.UserApi.Application.Services;

public class AuthService(IKeycloakAdminService keycloakAdminService, IUserProfileService userProfileService) : IAuthService
{
    public async Task<UserProfileResponse> RegisterAsync(RegisterRequest request)
    {
        var keycloakSubjectId = await keycloakAdminService.CreateUserAsync(
            new CreateKeycloakUserRequest(request.Username, request.Email, request.FirstName, request.LastName, request.Password),
            CancellationToken.None);

        return await userProfileService.CreateAsync(new CreateUserProfileRequest(
            keycloakSubjectId,
            $"{request.FirstName} {request.LastName}",
            request.Email));
    }
}
