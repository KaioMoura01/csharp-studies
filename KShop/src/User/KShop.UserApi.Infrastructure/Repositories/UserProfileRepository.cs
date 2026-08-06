using KShop.UserApi.Domain.Models;
using KShop.UserApi.Domain.Repositories;
using KShop.UserApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KShop.UserApi.Infrastructure.Repositories;

public class UserProfileRepository(AppDbContext context) : IUserProfileRepository
{
    public IQueryable<UserProfile> Query() => context.UserProfiles;

    public Task<UserProfile?> GetByIdAsync(Guid id) =>
        context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);

    public Task<UserProfile?> GetBySubAsync(string sub) =>
        context.UserProfiles.FirstOrDefaultAsync(u => u.KeycloakSubjectId == sub);

    public void Add(UserProfile userProfile) => context.UserProfiles.Add(userProfile);

    public void Remove(UserProfile userProfile) => context.UserProfiles.Remove(userProfile);
}
