namespace OrderService.Application.Abstractions;

public interface IUserServiceClient
{
    Task<RemoteUser?> GetUserAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<RemoteUser>> ListUsersAsync(CancellationToken cancellationToken);
}

public sealed record RemoteUser(string Id, string Name, string Email, bool Active);
