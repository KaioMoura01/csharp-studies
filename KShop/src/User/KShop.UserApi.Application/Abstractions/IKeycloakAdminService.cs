namespace KShop.UserApi.Application.Abstractions;

public interface IKeycloakAdminService
{
    Task<string> CreateUserAsync(CreateKeycloakUserRequest request, CancellationToken cancellationToken);
}

public record CreateKeycloakUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);
