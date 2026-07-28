using BankLedgerApi.DTOs.Common;

namespace BankLedgerApi.DTOs.Customers;

public record CustomerSummaryResponse(
    Guid Id,
    string Name,
    FiscalDocumentDto Document);
