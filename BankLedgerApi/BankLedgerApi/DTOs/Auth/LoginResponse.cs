namespace BankLedgerApi.DTOs.Auth;

public record LoginResponse(
    string Token,
    DateTimeOffset ExpiresAt);
