using BankLedgerApi.Application.DTOs.Accounts;

namespace BankLedgerApi.Application.DTOs.Auth;

public record LoginResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    Guid CustomerId,
    Guid ActiveAccountId,
    IReadOnlyList<AccountSummaryResponse> Accounts);
