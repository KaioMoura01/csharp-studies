using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Application.DTOs.Auth;
using BankLedgerApi.Application.Services;
using BankLedgerApi.Infrastructure.Security;
using AwesomeAssertions;
using Microsoft.Extensions.Options;

namespace BanLedgerApiXUnitTests.UnitTests;

public class AuthServiceTests
{
    private static AuthService CreateService(TestDatabase db)
    {
        var settings = Options.Create(new JwtSettings
        {
            Issuer = "BankLedgerApi",
            Audience = "BankLedgerApi",
            Key = "unit-test-signing-key-with-enough-length-1234567890",
            ExpiryMinutes = 60
        });

        var tokenGenerator = new JwtTokenGenerator(settings);
        return new AuthService(db.CustomerRepository, db.AccountRepository, db.PasswordHasher, tokenGenerator);
    }

    [Fact]
    public async Task LoginAsync_WhenCustomerMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("00000000000", "1234"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111", password: "correct");
        await db.SeedAccountAsync(customer.Id, "1111111111");
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("11111111111", "wrong"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenCustomerHasNoActiveAccount_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111", password: "secret");
        await db.SeedAccountAsync(customer.Id, "1111111111", active: false);
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("11111111111", "secret"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenScopedToFirstActiveAccount()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111", password: "secret");
        var firstAccount = await db.SeedAccountAsync(customer.Id, "1111111111", createdAt: DateTime.UtcNow.AddDays(-1));
        await db.SeedAccountAsync(customer.Id, "2222222222", createdAt: DateTime.UtcNow);
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("111.111.111-11", "secret"));

        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
        response.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        response.CustomerId.Should().Be(customer.Id);
        response.ActiveAccountId.Should().Be(firstAccount.Id);
        response.Accounts.Should().HaveCount(2);
    }

    [Fact]
    public async Task SwitchAccountAsync_WhenTargetBelongsToAnotherCustomer_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111");
        var otherCustomer = await db.SeedCustomerAsync(document: "22222222222");
        var otherAccount = await db.SeedAccountAsync(otherCustomer.Id, "9999999999");
        var service = CreateService(db);

        var response = await service.SwitchAccountAsync(customer.Id, otherAccount.Id);

        response.Should().BeNull();
    }

    [Fact]
    public async Task SwitchAccountAsync_WhenTargetIsInactive_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111");
        var inactiveAccount = await db.SeedAccountAsync(customer.Id, "1111111111", active: false);
        var service = CreateService(db);

        var response = await service.SwitchAccountAsync(customer.Id, inactiveAccount.Id);

        response.Should().BeNull();
    }

    [Fact]
    public async Task SwitchAccountAsync_WithOwnedActiveAccount_ReissuesTokenForThatAccount()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(document: "11111111111");
        var firstAccount = await db.SeedAccountAsync(customer.Id, "1111111111", createdAt: DateTime.UtcNow.AddDays(-1));
        var secondAccount = await db.SeedAccountAsync(customer.Id, "2222222222", createdAt: DateTime.UtcNow);
        var service = CreateService(db);

        var response = await service.SwitchAccountAsync(customer.Id, secondAccount.Id);

        response.Should().NotBeNull();
        response!.ActiveAccountId.Should().Be(secondAccount.Id);
        response.ActiveAccountId.Should().NotBe(firstAccount.Id);
        response.CustomerId.Should().Be(customer.Id);
    }
}
