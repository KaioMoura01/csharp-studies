namespace BankLedgerApi.Domain.Repositories;

public interface ITransactionScope : IAsyncDisposable
{
    Task CommitAsync();
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task<ITransactionScope> BeginTransactionAsync();
}
