namespace LibraryApi.DTOs.Request;

public record CreateUserRequest(
    string Name,
    string Email,
    string Password
);
