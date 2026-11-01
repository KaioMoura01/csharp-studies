using TodoForSso.TodoApi.Application.Dtos;

namespace TodoForSso.TodoApi.Application.Services;

public interface IUserAdminService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task<List<UserDto>> GetAllAsync();
    Task<bool> SetEnabledAsync(string userId, bool enabled);
}
