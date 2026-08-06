using KShop.UserApi.Application.DTOs.Auth;
using KShop.UserApi.Application.DTOs.UserProfiles;

namespace KShop.UserApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<UserProfileResponse> RegisterAsync(RegisterRequest request);
}
