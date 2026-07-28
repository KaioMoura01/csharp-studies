using BankLedgerApi.DTOs.Common;
using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Transfers;

public record TransferResponse(
    Guid Id,
    Guid TransactionId,
    AccountRefDto Source,
    AccountRefDto Destination,
    decimal Amount,
    TransferStatusEnum Status,
    DateTimeOffset CreatedAt);
