using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Accounts;

public record AccountSummaryResponse(
    Guid Id,
    string Name,
    string Number,
    AccountTypeEnum Type,
    decimal CurrentBalance,
    bool IsActive);
