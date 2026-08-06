namespace UserService.Application.Users;

public sealed class UserNotFoundException(string id) : Exception($"Usuário '{id}' não encontrado");
