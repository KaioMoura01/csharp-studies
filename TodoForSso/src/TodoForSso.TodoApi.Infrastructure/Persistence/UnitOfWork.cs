using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public ITodoRepository Todos { get; } = new TodoRepository(context);

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
