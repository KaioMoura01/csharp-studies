using AppPasswordVerificationResult = BankLedgerApi.Application.Security.PasswordVerificationResult;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.PasswordHasher<BankLedgerApi.Infrastructure.Security.PasswordHasherAdapter.HashSubject>;
using IdentityVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace BankLedgerApi.Infrastructure.Security;

public class PasswordHasherAdapter : Application.Security.IPasswordHasher
{
    // ASP.NET Identity's PasswordHasher<TUser> only uses TUser for its generic signature, never its state.
    public sealed class HashSubject;

    private readonly IdentityPasswordHasher _hasher = new();

    public string HashPassword(string password) =>
        _hasher.HashPassword(new HashSubject(), password);

    public AppPasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(new HashSubject(), hashedPassword, providedPassword);
        return result == IdentityVerificationResult.Failed
            ? AppPasswordVerificationResult.Failed
            : AppPasswordVerificationResult.Success;
    }
}
