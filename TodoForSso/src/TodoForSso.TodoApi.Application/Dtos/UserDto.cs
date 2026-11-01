namespace TodoForSso.TodoApi.Application.Dtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Enabled { get; set; }
    public List<string> Roles { get; set; } = [];
}

public class RegisterUserDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class UpdateUserStatusDto
{
    public required bool Enabled { get; set; }
}
