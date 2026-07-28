using BankLedgerApi.DTOs.Common;
using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Accounts;

public record AccountDetailsResponse(
    Guid Id,
    string Name,
    string Number,
    AccountTypeEnum Type,
    decimal CurrentBalance,
    bool IsActive,
    CustomerRefDto Owner);
