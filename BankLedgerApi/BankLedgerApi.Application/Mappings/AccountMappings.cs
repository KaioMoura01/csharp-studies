using BankLedgerApi.Application.DTOs.Accounts;
using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Application.Mappings;

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
