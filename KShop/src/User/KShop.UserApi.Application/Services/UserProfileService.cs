using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Application.Services.Interfaces;
using KShop.UserApi.Domain.Models;
using KShop.UserApi.Domain.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace KShop.UserApi.Application.Services;

public class UserProfileService(IUnitOfWork unitOfWork) : IUserProfileService
{
    public async Task<List<UserProfileResponse>> GetAllAsync()
    {
        return await unitOfWork.UserProfiles.Query()
            .ProjectToType<UserProfileResponse>()
            .ToListAsync();
    }

    public async Task<UserProfileResponse?> GetByIdAsync(Guid id)
    {
        var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(id);
        return userProfile?.Adapt<UserProfileResponse>();
    }

    public async Task<UserProfileResponse?> GetBySubAsync(string sub)
    {
        var userProfile = await unitOfWork.UserProfiles.GetBySubAsync(sub);
        return userProfile?.Adapt<UserProfileResponse>();
    }

    public async Task<UserProfileResponse> CreateAsync(CreateUserProfileRequest request)
    {
        var userProfile = request.Adapt<UserProfile>();

        unitOfWork.UserProfiles.Add(userProfile);
        await unitOfWork.SaveChangesAsync();

        var created = await unitOfWork.UserProfiles.GetByIdAsync(userProfile.Id);
        return created!.Adapt<UserProfileResponse>();
    }

    public async Task<UserProfileResponse?> UpdateAsync(Guid id, UpdateUserProfileRequest request)
    {
        var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(id);
        if (userProfile is null) return null;

        request.Adapt(userProfile);
        await unitOfWork.SaveChangesAsync();

        return userProfile.Adapt<UserProfileResponse>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(id);
        if (userProfile is null) return false;

        unitOfWork.UserProfiles.Remove(userProfile);
        await unitOfWork.SaveChangesAsync();
        return true;
    }
}
