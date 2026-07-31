using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Application.DTOs.Transfers;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Application.Services;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace BanLedgerApiXUnitTests.UnitTests;

public class TransferServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WithNonPositiveAmount_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("2222222222", 0m, "1234"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenDestinationMissing_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("0000000000", 10m, "1234"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSourceEqualsDestination_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("1111111111", 10m, "1234"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenDestinationInactive_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        await db.SeedAccountAsync(customer.Id, "2222222222", active: false);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("2222222222", 10m, "1234"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithInsufficientBalance_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 5m);
        await db.SeedAccountAsync(customer.Id, "2222222222", balance: 0m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("2222222222", 10m, "1234"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithWrongPassword_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(password: "correct");
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        await db.SeedAccountAsync(customer.Id, "2222222222", balance: 0m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ExecuteAsync(source.Id, new CreateTransferRequest("2222222222", 10m, "wrong"));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_MovesBalancesAndRecordsCompletedTransfer()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 20m);
        var service = new TransferService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var response = await service.ExecuteAsync(source.Id, new CreateTransferRequest("2222222222", 30m, "1234"));

        response.Amount.Should().Be(30m);
        response.Status.Should().Be(TransferStatusEnum.Completed);
        response.Source.Number.Should().Be("1111111111");
        response.Destination.Number.Should().Be("2222222222");

        var reloadedSource = await db.Context.Accounts.SingleAsync(a => a.Id == source.Id);
        var reloadedDestination = await db.Context.Accounts.SingleAsync(a => a.Id == destination.Id);
        reloadedSource.CurrentBalance.Should().Be(70m);
        reloadedDestination.CurrentBalance.Should().Be(50m);
    }
}
