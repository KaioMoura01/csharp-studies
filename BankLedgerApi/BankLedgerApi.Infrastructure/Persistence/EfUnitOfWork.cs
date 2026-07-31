using BankLedgerApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankLedgerApi.Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => dbContext.SaveChangesAsync();

    public async Task<ITransactionScope> BeginTransactionAsync()
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        return new EfTransactionScope(transaction);
    }

    private sealed class EfTransactionScope(IDbContextTransaction transaction) : ITransactionScope
    {
        public Task CommitAsync() => transaction.CommitAsync();

        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
