using BankLedgerApi.Application.DTOs.Common;

namespace BankLedgerApi.Application.DTOs.Reversals;

public record ReversalResponse(
    Guid OriginalTransferId,
    Guid ReversalTransactionId,
    AccountRefDto From,
    AccountRefDto To,
    decimal Amount,
    DateTimeOffset CreatedAt);
