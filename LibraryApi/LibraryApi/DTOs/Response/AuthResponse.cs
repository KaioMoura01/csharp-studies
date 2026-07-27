namespace LibraryApi.DTOs.Response;

public record AuthResponse(string Token, DateTime ExpiresAt);
