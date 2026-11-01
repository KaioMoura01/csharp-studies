namespace TodoForSso.TodoApi.Domain;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, string ownerId);
    Task<List<TodoItem>> GetAllByOwnerAsync(string ownerId);
    Task AddAsync(TodoItem todo);
    void Update(TodoItem todo);
    void Remove(TodoItem todo);
}
