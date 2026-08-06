using UserService.Application.Abstractions;

namespace UserService.Application.Users;

public sealed class GetUserHandler(IUserRepository repository)
{
    public async Task<UserDto> HandleAsync(string id, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new UserNotFoundException(id);

        return new UserDto(user.Id, user.Name, user.Email, user.Active);
    }
}
