using LibraryApi.DTOs.Request;
using LibraryApi.DTOs.Response;
using LibraryApi.Enums;
using LibraryApi.Models;

namespace LibraryApi.Extensions;

public static class UserMappingExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role);

    public static IEnumerable<UserResponse> ToResponse(this IEnumerable<User> users)
        => users.Select(u => u.ToResponse());

    public static User ToEntity(this CreateUserRequest request, string passwordHash, Role role) => new()
    {
        Name = request.Name,
        Email = request.Email,
        PasswordHash = passwordHash,
        Role = role
    };
}
