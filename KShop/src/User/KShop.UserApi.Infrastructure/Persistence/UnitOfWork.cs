using KShop.UserApi.Domain.Repositories;

namespace KShop.UserApi.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context, IUserProfileRepository userProfiles) : IUnitOfWork
{
    public IUserProfileRepository UserProfiles { get; } = userProfiles;

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
