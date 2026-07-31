using BankLedgerApi.DTOs.Common;

namespace BankLedgerApi.DTOs.Reversals;

public record ReversalResponse(
    Guid OriginalTransferId,
    Guid ReversalTransactionId,
    AccountRefDto From,
    AccountRefDto To,
    decimal Amount,
    DateTimeOffset CreatedAt);
