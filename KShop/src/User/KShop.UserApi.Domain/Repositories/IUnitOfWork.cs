namespace KShop.UserApi.Domain.Repositories;

public interface IUnitOfWork
{
    IUserProfileRepository UserProfiles { get; }
    Task<int> SaveChangesAsync();
}
