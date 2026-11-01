using Microsoft.EntityFrameworkCore;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Persistence;

public class TodoRepository(AppDbContext context) : ITodoRepository
{
    public Task<TodoItem?> GetByIdAsync(Guid id, string ownerId) =>
        context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId);

    public Task<List<TodoItem>> GetAllByOwnerAsync(string ownerId) =>
        context.Todos.Where(t => t.OwnerId == ownerId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public Task AddAsync(TodoItem todo) => context.Todos.AddAsync(todo).AsTask();

    public void Update(TodoItem todo) => context.Todos.Update(todo);

    public void Remove(TodoItem todo) => context.Todos.Remove(todo);
}
