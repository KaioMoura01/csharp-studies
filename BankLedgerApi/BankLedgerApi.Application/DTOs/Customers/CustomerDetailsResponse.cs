using BankLedgerApi.Application.DTOs.Accounts;
using BankLedgerApi.Application.DTOs.Common;

namespace BankLedgerApi.Application.DTOs.Customers;

public record CustomerDetailsResponse(
    Guid Id,
    string Name,
    FiscalDocumentDto Document,
    IReadOnlyList<AccountSummaryResponse> Accounts);
