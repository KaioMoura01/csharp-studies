namespace BankLedgerApi.Application.DTOs.Reversals;

public record ReversalRequest(
    Guid TransferId,
    string Password);
