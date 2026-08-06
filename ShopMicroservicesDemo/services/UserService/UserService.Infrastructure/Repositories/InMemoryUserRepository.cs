using System.Collections.Concurrent;
using UserService.Application.Abstractions;
using UserService.Domain;

namespace UserService.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public InMemoryUserRepository()
    {
        Seed(new User { Id = "1", Name = "Ana Silva", Email = "ana@example.com", Active = true });
        Seed(new User { Id = "2", Name = "Bruno Costa", Email = "bruno@example.com", Active = false });
        Seed(new User { Id = "3", Name = "Carla Souza", Email = "carla@example.com", Active = true });
    }

    public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<User> all = _users.Values.ToList();
        return Task.FromResult(all);
    }

    private void Seed(User user) => _users[user.Id] = user;
}
