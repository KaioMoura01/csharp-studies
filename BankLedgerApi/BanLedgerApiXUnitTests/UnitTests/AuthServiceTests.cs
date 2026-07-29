using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Configurations;
using BankLedgerApi.DTOs.Auth;
using BankLedgerApi.Services;
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

        return new AuthService(db.Context, db.PasswordHasher, settings);
    }

    [Fact]
    public async Task LoginAsync_WhenAccountMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("0000000000", "1234"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        await db.SeedAccountAsync(customer.Id, "1111111111", password: "correct");
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("1111111111", "wrong"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenAccountInactive_ReturnsNull()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        await db.SeedAccountAsync(customer.Id, "1111111111", password: "secret", active: false);
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("1111111111", "secret"));

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenWithFutureExpiry()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        await db.SeedAccountAsync(customer.Id, "1111111111", password: "secret");
        var service = CreateService(db);

        var response = await service.LoginAsync(new LoginRequest("1111111111", "secret"));

        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
        response.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }
}
