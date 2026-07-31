namespace BankLedgerApi.Application.DTOs.Auth;

public record LoginRequest(
    string DocumentNumber,
    string Password);
