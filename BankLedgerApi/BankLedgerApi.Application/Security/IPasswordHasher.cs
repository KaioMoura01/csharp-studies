namespace BankLedgerApi.Application.Security;

public enum PasswordVerificationResult
{
    Failed,
    Success
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyPassword(string hashedPassword, string providedPassword);
}
