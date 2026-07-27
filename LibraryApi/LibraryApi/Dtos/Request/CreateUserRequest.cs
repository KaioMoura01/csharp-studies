namespace LibraryApi.Dtos.Request;

public record CreateUserRequest(
    string Name,
    string Email,
    string Password
);
