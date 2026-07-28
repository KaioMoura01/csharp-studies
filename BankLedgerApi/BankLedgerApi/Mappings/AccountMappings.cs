using BankLedgerApi.DTOs.Accounts;
using BankLedgerApi.DTOs.Common;
using BankLedgerApi.Models;

namespace BankLedgerApi.Mappings;

public static class AccountMappings
{
    public static AccountSummaryResponse ToSummary(this Account account) =>
        new(account.Id,
            account.Name,
            account.Number,
            account.Type,
            account.CurrentBalance,
            account.IsActive);

    public static AccountDetailsResponse ToDetails(this Account account) =>
        new(account.Id,
            account.Name,
            account.Number,
            account.Type,
            account.CurrentBalance,
            account.IsActive,
            new CustomerRefDto(account.Customer.Id, account.Customer.Name));

    public static AccountCreatedResponse ToCreated(this Account account) =>
        new(account.Id,
            account.Number,
            account.Name,
            account.Type);
}
