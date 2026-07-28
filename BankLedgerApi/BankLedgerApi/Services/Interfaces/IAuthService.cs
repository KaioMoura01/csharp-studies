using BankLedgerApi.DTOs.Auth;

namespace BankLedgerApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
