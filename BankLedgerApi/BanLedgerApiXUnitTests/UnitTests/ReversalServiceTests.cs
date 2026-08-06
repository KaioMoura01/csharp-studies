using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Application.Services;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace BanLedgerApiXUnitTests.UnitTests;

public class ReversalServiceTests
{
    private static async Task<Transfer> SeedCompletedTransferAsync(
        TestDatabase db, Guid sourceId, Guid destinationId, decimal amount)
    {
        var transfer = new Transfer
        {
            TenantId = db.TenantId,
            SourceAccountId = sourceId,
            DestinationAccountId = destinationId,
            Amount = amount,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow
        };

        db.Context.Transfers.Add(transfer);
        await db.Context.SaveChangesAsync();
        return transfer;
    }

    [Fact]
    public async Task ReverseAsync_WhenTransferMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var response = await service.ReverseAsync(Guid.NewGuid(), Guid.NewGuid(), "1234");

        response.Should().BeNull();
    }

    [Fact]
    public async Task ReverseAsync_WhenDeposit_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var account = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 100m);
        var deposit = new Transfer
        {
            TenantId = db.TenantId,
            SourceAccountId = null,
            DestinationAccountId = account.Id,
            Amount = 100m,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow
        };
        db.Context.Transfers.Add(deposit);
        await db.Context.SaveChangesAsync();
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ReverseAsync(account.Id, deposit.Id, "1234");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReverseAsync_WhenCallerIsNotSource_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 70m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 30m);
        var transfer = await SeedCompletedTransferAsync(db, source.Id, destination.Id, 30m);
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ReverseAsync(destination.Id, transfer.Id, "1234");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReverseAsync_WhenDestinationHasInsufficientBalance_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 70m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 10m);
        var transfer = await SeedCompletedTransferAsync(db, source.Id, destination.Id, 30m);
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ReverseAsync(source.Id, transfer.Id, "1234");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReverseAsync_WithWrongPassword_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync(password: "correct");
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 70m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 30m);
        var transfer = await SeedCompletedTransferAsync(db, source.Id, destination.Id, 30m);
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var act = () => service.ReverseAsync(source.Id, transfer.Id, "wrong");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ReverseAsync_WithValidData_MovesMoneyBackAndRecordsCompensatingEntry()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 70m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 30m);
        var transfer = await SeedCompletedTransferAsync(db, source.Id, destination.Id, 30m);
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        var response = await service.ReverseAsync(source.Id, transfer.Id, "1234");

        response.Should().NotBeNull();
        response!.OriginalTransferId.Should().Be(transfer.Id);
        response.Amount.Should().Be(30m);

        var reloadedSource = await db.Context.Accounts.SingleAsync(a => a.Id == source.Id);
        var reloadedDestination = await db.Context.Accounts.SingleAsync(a => a.Id == destination.Id);
        reloadedSource.CurrentBalance.Should().Be(100m);
        reloadedDestination.CurrentBalance.Should().Be(0m);

        var compensating = await db.Context.Transfers.SingleAsync(t => t.ReversedTransferId == transfer.Id);
        compensating.SourceAccountId.Should().Be(destination.Id);
        compensating.DestinationAccountId.Should().Be(source.Id);
    }

    [Fact]
    public async Task ReverseAsync_WhenAlreadyReversed_Throws()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var source = await db.SeedAccountAsync(customer.Id, "1111111111", balance: 70m);
        var destination = await db.SeedAccountAsync(customer.Id, "2222222222", balance: 30m);
        var transfer = await SeedCompletedTransferAsync(db, source.Id, destination.Id, 30m);
        var service = new ReversalService(db.AccountRepository, db.TransferRepository, db.CustomerRepository, db.PasswordHasher, db.UnitOfWork);

        await service.ReverseAsync(source.Id, transfer.Id, "1234");

        var act = () => service.ReverseAsync(source.Id, transfer.Id, "1234");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
