namespace TodoForSso.TodoApi.Application.Abstractions;

public interface ICurrentUserContext
{
    string UserId { get; }
    string? Username { get; }
    bool IsAdmin { get; }
}
