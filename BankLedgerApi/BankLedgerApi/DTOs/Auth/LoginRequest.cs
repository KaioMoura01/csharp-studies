namespace BankLedgerApi.DTOs.Auth;

public record LoginRequest(
    string AccountNumber,
    string Password);
