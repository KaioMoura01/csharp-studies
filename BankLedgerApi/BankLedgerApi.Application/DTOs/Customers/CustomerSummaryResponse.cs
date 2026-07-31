using BankLedgerApi.Application.DTOs.Common;

namespace BankLedgerApi.Application.DTOs.Customers;

public record CustomerSummaryResponse(
    Guid Id,
    string Name,
    FiscalDocumentDto Document);
