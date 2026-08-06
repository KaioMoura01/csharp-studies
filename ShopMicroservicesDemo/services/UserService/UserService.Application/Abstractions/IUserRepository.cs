using UserService.Domain;

namespace UserService.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken);
}
