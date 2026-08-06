namespace KShop.UserApi.Application.DTOs.Auth;

public record RegisterRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);
