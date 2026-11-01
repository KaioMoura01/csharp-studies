namespace TodoForSso.TodoApi.Domain;

public interface IUnitOfWork
{
    ITodoRepository Todos { get; }
    Task<int> SaveChangesAsync();
}
