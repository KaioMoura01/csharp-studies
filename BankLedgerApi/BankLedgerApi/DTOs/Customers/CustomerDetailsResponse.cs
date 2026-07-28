using BankLedgerApi.DTOs.Accounts;
using BankLedgerApi.DTOs.Common;

namespace BankLedgerApi.DTOs.Customers;

public record CustomerDetailsResponse(
    Guid Id,
    string Name,
    FiscalDocumentDto Document,
    IReadOnlyList<AccountSummaryResponse> Accounts);
