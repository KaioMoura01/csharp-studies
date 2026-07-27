namespace LibraryApi.Dtos.Request;

public record LoginRequest(
    string Email,
    string Password
);
