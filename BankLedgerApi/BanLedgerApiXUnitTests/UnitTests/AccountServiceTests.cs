using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Application.DTOs.Accounts;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Application.Services;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace BanLedgerApiXUnitTests.UnitTests;

public class AccountServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenCustomerMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.CreateAsync(
            new CreateAccountRequest(Guid.NewGuid(), "Main", AccountTypeEnum.Checking));

        response.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidCustomer_CreatesActiveAccount()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.CreateAsync(
            new CreateAccountRequest(customer.Id, "Main", AccountTypeEnum.Savings));

        response.Should().NotBeNull();
        response!.Number.Should().HaveLength(10);

        var stored = await db.Context.Accounts.SingleAsync(a => a.Id == response.Id);
        stored.IsActive.Should().BeTrue();
        stored.CurrentBalance.Should().Be(0m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDetailsWithOwner()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(name: "Maria");
        var account = await db.SeedAccountAsync(customer.Id, "2000000002");
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.GetByIdAsync(account.Id);

        response.Should().NotBeNull();
        response!.Number.Should().Be("2000000002");
        response.Owner.Id.Should().Be(customer.Id);
        response.Owner.Name.Should().Be("Maria");
    }

    [Fact]
    public async Task DepositAsync_WithNonPositiveAmount_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var account = await db.SeedAccountAsync(customer.Id, "3000000003");
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var act = () => service.DepositAsync(account.Id, 0m);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DepositAsync_WhenAccountMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.DepositAsync(Guid.NewGuid(), 100m);

        response.Should().BeNull();
    }

    [Fact]
    public async Task DepositAsync_WhenInactive_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var account = await db.SeedAccountAsync(customer.Id, "4000000004", active: false);
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var act = () => service.DepositAsync(account.Id, 100m);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DepositAsync_WithValidAmount_CreditsBalanceAndRecordsLedgerEntry()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var account = await db.SeedAccountAsync(customer.Id, "5000000005", balance: 100m);
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.DepositAsync(account.Id, 250m);

        response!.CurrentBalance.Should().Be(350m);

        var entry = await db.Context.Transfers.SingleAsync(t => t.DestinationAccountId == account.Id);
        entry.SourceAccountId.Should().BeNull();
        entry.Amount.Should().Be(250m);
        entry.Status.Should().Be(TransferStatusEnum.Completed);
    }

    [Fact]
    public async Task GetByCustomerAsync_ReturnsOnlyThatCustomerAccounts()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var other = await db.SeedCustomerAsync(document: "98765432100");
        await db.SeedAccountAsync(customer.Id, "6000000006");
        await db.SeedAccountAsync(customer.Id, "6000000007");
        await db.SeedAccountAsync(other.Id, "6000000008");
        var service = new AccountService(db.AccountRepository, db.TransferRepository, db.UnitOfWork);

        var response = await service.GetByCustomerAsync(customer.Id);

        response.Should().HaveCount(2);
        response.Should().OnlyContain(a => a.Number.StartsWith("60000000"));
    }
}
