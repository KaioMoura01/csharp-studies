using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Domain.Repositories;

public interface ITransferRepository
{
    void Add(Transfer transfer);
    Task<Transfer?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Transfer>> GetActivityBeforeAsync(Guid accountId, DateTime beforeUtc);
    Task<IReadOnlyList<Transfer>> GetActivityInRangeAsync(Guid accountId, DateTime fromInclusiveUtc, DateTime toExclusiveUtc);
}
