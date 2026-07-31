using BankLedgerApi.DTOs.Reversals;

namespace BankLedgerApi.Services.Interfaces;

public interface IReversalService
{
    Task<ReversalResponse?> ReverseAsync(Guid callerAccountId, Guid transferId);
}
