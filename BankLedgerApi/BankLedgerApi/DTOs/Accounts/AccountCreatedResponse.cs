using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Accounts;

public record AccountCreatedResponse(
    Guid Id,
    string Number,
    string Name,
    AccountTypeEnum Type);
