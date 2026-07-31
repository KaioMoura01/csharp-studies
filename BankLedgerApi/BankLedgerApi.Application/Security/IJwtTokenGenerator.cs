namespace BankLedgerApi.Application.Security;

public record GeneratedToken(string Token, DateTimeOffset ExpiresAt);

public interface IJwtTokenGenerator
{
    GeneratedToken Generate(Guid activeAccountId, Guid customerId);
}
