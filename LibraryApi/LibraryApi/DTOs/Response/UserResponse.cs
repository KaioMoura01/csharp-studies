using LibraryApi.Enums;

namespace LibraryApi.DTOs.Response;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    Role Role
);
