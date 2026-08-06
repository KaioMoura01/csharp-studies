namespace UserService.Application.Users;

public sealed record UserDto(string Id, string Name, string Email, bool Active);
