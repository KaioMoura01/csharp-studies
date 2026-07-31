using BankLedgerApi.Application.DTOs.Auth;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> SwitchAccountAsync(Guid callerCustomerId, Guid targetAccountId);
}
