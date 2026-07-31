using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;
using BankLedgerApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Infrastructure.Repositories;

public class TransferRepository(AppDbContext dbContext) : ITransferRepository
{
    public void Add(Transfer transfer) => dbContext.Transfers.Add(transfer);

    public Task<Transfer?> GetByIdAsync(Guid id) =>
        dbContext.Transfers.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IReadOnlyList<Transfer>> GetActivityBeforeAsync(Guid accountId, DateTime beforeUtc) =>
        await dbContext.Transfers
            .Where(t => (t.Status == TransferStatusEnum.Completed || t.Status == TransferStatusEnum.Reversed)
                        && t.CreatedAt < beforeUtc
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .ToListAsync();

    public async Task<IReadOnlyList<Transfer>> GetActivityInRangeAsync(Guid accountId, DateTime fromInclusiveUtc, DateTime toExclusiveUtc) =>
        await dbContext.Transfers
            .Where(t => (t.Status == TransferStatusEnum.Completed || t.Status == TransferStatusEnum.Reversed)
                        && t.CreatedAt >= fromInclusiveUtc
                        && t.CreatedAt < toExclusiveUtc
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
}
