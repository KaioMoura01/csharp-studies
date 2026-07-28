using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Accounts;

public record CreateAccountRequest(
    Guid CustomerId,
    string Name,
    AccountTypeEnum Type,
    string Password);
