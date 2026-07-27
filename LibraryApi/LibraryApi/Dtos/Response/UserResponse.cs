using LibraryApi.Enums;

namespace LibraryApi.Dtos.Response;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    Role Role
);
