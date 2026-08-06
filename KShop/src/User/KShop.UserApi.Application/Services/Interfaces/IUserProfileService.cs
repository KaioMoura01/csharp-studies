using KShop.UserApi.Application.DTOs.UserProfiles;

namespace KShop.UserApi.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<List<UserProfileResponse>> GetAllAsync();
    Task<UserProfileResponse?> GetByIdAsync(Guid id);
    Task<UserProfileResponse?> GetBySubAsync(string sub);
    Task<UserProfileResponse> CreateAsync(CreateUserProfileRequest request);
    Task<UserProfileResponse?> UpdateAsync(Guid id, UpdateUserProfileRequest request);
    Task<bool> DeleteAsync(Guid id);
}
