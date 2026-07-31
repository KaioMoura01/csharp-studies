using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Accounts;

public record AccountCreatedResponse(
    Guid Id,
    string Number,
    string Name,
    AccountTypeEnum Type);
