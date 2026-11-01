using TodoForSso.TodoApi.Application.Dtos;

namespace TodoForSso.TodoApi.Application.Services;

public interface ITodoService
{
    Task<List<TodoDto>> GetAllAsync();
    Task<TodoDto?> GetByIdAsync(Guid id);
    Task<TodoDto> CreateAsync(CreateTodoDto dto);
    Task<TodoDto?> UpdateAsync(Guid id, UpdateTodoDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<TodoDto?> CompleteAsync(Guid id);
    Task<TodoDto?> ReopenAsync(Guid id);
}
