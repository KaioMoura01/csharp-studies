using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Accounts;

public record CreateAccountRequest(
    Guid CustomerId,
    string Name,
    AccountTypeEnum Type);
