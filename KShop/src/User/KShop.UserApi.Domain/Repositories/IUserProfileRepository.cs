using KShop.UserApi.Domain.Models;

namespace KShop.UserApi.Domain.Repositories;

public interface IUserProfileRepository
{
    IQueryable<UserProfile> Query();
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<UserProfile?> GetBySubAsync(string sub);
    void Add(UserProfile userProfile);
    void Remove(UserProfile userProfile);
}
