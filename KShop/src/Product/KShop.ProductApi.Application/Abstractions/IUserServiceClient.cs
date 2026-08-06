namespace KShop.ProductApi.Application.Abstractions;

public interface IUserServiceClient
{
    Task<RemoteUserProfile?> GetUserBySubAsync(string sub, CancellationToken cancellationToken);
}

public sealed record RemoteUserProfile(Guid Id, string KeycloakSubjectId, string DisplayName, string Email, bool Active);
