using BankLedgerApi.Application.DTOs.Reversals;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface IReversalService
{
    Task<ReversalResponse?> ReverseAsync(Guid callerAccountId, Guid transferId, string password);
}
