using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Transfers;

public record TransferResponse(
    Guid Id,
    Guid TransactionId,
    AccountRefDto Source,
    AccountRefDto Destination,
    decimal Amount,
    TransferStatusEnum Status,
    DateTimeOffset CreatedAt);
