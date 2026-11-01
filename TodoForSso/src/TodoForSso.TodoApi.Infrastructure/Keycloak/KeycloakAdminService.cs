using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.Extensions.Options;
using TodoForSso.TodoApi.Application.Dtos;
using TodoForSso.TodoApi.Application.Services;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Keycloak;

public class KeycloakAdminService : IUserAdminService
{
    private readonly KeycloakClient _client;
    private readonly KeycloakAdminOptions _options;

    public KeycloakAdminService(IOptions<KeycloakAdminOptions> options, KeycloakAdminTokenProvider tokenProvider)
    {
        _options = options.Value;
        _client = new KeycloakClient(_options.AdminUrl, tokenProvider.GetToken);
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Enabled = true,
            Credentials = new List<Credentials>
            {
                new() { Type = "password", Value = dto.Password, Temporary = false },
            },
        };

        var userId = await _client.CreateAndRetrieveUserIdAsync(_options.Realm, user)
            ?? throw new DomainException("Keycloak não retornou o id do usuário criado.");

        var role = await _client.GetRoleByNameAsync(_options.Realm, _options.DefaultRole);
        if (role is not null)
        {
            await _client.AddRealmRoleMappingsToUserAsync(_options.Realm, userId, [role]);
        }

        return new UserDto
        {
            Id = userId,
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Enabled = true,
            Roles = [_options.DefaultRole],
        };
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _client.GetUsersAsync(_options.Realm, max: 200);

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            if (user.Id is null)
            {
                continue;
            }

            var roles = await _client.GetRealmRoleMappingsForUserAsync(_options.Realm, user.Id);
            result.Add(new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Enabled = user.Enabled ?? false,
                Roles = roles.Where(r => r.Name is not null).Select(r => r.Name!).ToList(),
            });
        }

        return result;
    }

    public async Task<bool> SetEnabledAsync(string userId, bool enabled)
    {
        var user = await _client.GetUserAsync(_options.Realm, userId);
        if (user is null)
        {
            return false;
        }

        user.Enabled = enabled;
        return await _client.UpdateUserAsync(_options.Realm, userId, user);
    }
}
