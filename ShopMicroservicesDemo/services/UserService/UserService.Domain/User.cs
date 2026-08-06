namespace UserService.Domain;

public sealed class User
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required bool Active { get; init; }
}
