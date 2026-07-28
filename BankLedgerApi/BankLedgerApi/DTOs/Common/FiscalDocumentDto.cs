using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Common;

public record FiscalDocumentDto(
    string Number,
    DocumentTypeEnum Type);
