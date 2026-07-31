using BankLedgerApi.Application.DTOs.Auth;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class AuthService(
    ICustomerRepository customerRepository,
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var documentNumber = TaxDocument.NormalizeDigits(request.DocumentNumber);
        var customer = await customerRepository.GetByDocumentNumberAsync(documentNumber);
        if (customer is null)
            return null;

        var verification = passwordHasher.VerifyPassword(customer.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            return null;

        var accounts = await accountRepository.GetByCustomerAsync(customer.Id);
        var activeAccount = accounts
            .Where(a => a.IsActive)
            .OrderBy(a => a.CreatedAt)
            .FirstOrDefault();

        if (activeAccount is null)
            return null;

        return BuildResponse(customer.Id, activeAccount.Id, accounts);
    }

    public async Task<LoginResponse?> SwitchAccountAsync(Guid callerCustomerId, Guid targetAccountId)
    {
        var targetAccount = await accountRepository.GetByIdAsync(targetAccountId);
        if (targetAccount is null || targetAccount.CustomerId != callerCustomerId || !targetAccount.IsActive)
            return null;

        var accounts = await accountRepository.GetByCustomerAsync(callerCustomerId);

        return BuildResponse(callerCustomerId, targetAccount.Id, accounts);
    }

    private LoginResponse BuildResponse(Guid customerId, Guid activeAccountId, IReadOnlyList<Account> accounts)
    {
        var generated = tokenGenerator.Generate(activeAccountId, customerId);

        return new LoginResponse(
            generated.Token,
            generated.ExpiresAt,
            customerId,
            activeAccountId,
            accounts.Select(a => a.ToSummary()).ToList());
    }
}
