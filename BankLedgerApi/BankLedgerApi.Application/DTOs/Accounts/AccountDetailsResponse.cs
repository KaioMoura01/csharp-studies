using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Accounts;

public record AccountDetailsResponse(
    Guid Id,
    string Name,
    string Number,
    AccountTypeEnum Type,
    decimal CurrentBalance,
    bool IsActive,
    CustomerRefDto Owner);
