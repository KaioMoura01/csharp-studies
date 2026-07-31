using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Common;

public record FiscalDocumentDto(
    string Number,
    DocumentTypeEnum Type);
