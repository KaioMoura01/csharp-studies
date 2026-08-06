using UserService.Application.Abstractions;

namespace UserService.Application.Users;

public sealed class ListUsersHandler(IUserRepository repository)
{
    public async Task<IReadOnlyCollection<UserDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var users = await repository.GetAllAsync(cancellationToken);
        return users.Select(u => new UserDto(u.Id, u.Name, u.Email, u.Active)).ToList();
    }
}
