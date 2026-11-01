using Mapster;
using TodoForSso.TodoApi.Application.Abstractions;
using TodoForSso.TodoApi.Application.Dtos;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Application.Services;

public class TodoService(IUnitOfWork unitOfWork, ICurrentUserContext currentUser) : ITodoService
{
    public async Task<List<TodoDto>> GetAllAsync()
    {
        var todos = await unitOfWork.Todos.GetAllByOwnerAsync(currentUser.UserId);
        return todos.Adapt<List<TodoDto>>();
    }

    public async Task<TodoDto?> GetByIdAsync(Guid id)
    {
        var todo = await unitOfWork.Todos.GetByIdAsync(id, currentUser.UserId);
        return todo?.Adapt<TodoDto>();
    }

    public async Task<TodoDto> CreateAsync(CreateTodoDto dto)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            OwnerId = currentUser.UserId,
        };

        await unitOfWork.Todos.AddAsync(todo);
        await unitOfWork.SaveChangesAsync();

        return todo.Adapt<TodoDto>();
    }

    public async Task<TodoDto?> UpdateAsync(Guid id, UpdateTodoDto dto)
    {
        var todo = await unitOfWork.Todos.GetByIdAsync(id, currentUser.UserId);
        if (todo is null)
        {
            return null;
        }

        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.Priority = dto.Priority;
        todo.Status = dto.Status;
        todo.DueDate = dto.DueDate;
        todo.CompletedAt = dto.Status == TodoStatus.Done ? todo.CompletedAt ?? DateTime.UtcNow : null;

        unitOfWork.Todos.Update(todo);
        await unitOfWork.SaveChangesAsync();

        return todo.Adapt<TodoDto>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var todo = await unitOfWork.Todos.GetByIdAsync(id, currentUser.UserId);
        if (todo is null)
        {
            return false;
        }

        unitOfWork.Todos.Remove(todo);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<TodoDto?> CompleteAsync(Guid id)
    {
        var todo = await unitOfWork.Todos.GetByIdAsync(id, currentUser.UserId);
        if (todo is null)
        {
            return null;
        }

        todo.Complete();
        unitOfWork.Todos.Update(todo);
        await unitOfWork.SaveChangesAsync();

        return todo.Adapt<TodoDto>();
    }

    public async Task<TodoDto?> ReopenAsync(Guid id)
    {
        var todo = await unitOfWork.Todos.GetByIdAsync(id, currentUser.UserId);
        if (todo is null)
        {
            return null;
        }

        todo.Reopen();
        unitOfWork.Todos.Update(todo);
        await unitOfWork.SaveChangesAsync();

        return todo.Adapt<TodoDto>();
    }
}
