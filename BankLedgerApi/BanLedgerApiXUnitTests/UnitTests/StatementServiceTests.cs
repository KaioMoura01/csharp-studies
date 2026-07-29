using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.DTOs.Statements;
using BankLedgerApi.Enums;
using BankLedgerApi.Models;
using BankLedgerApi.Services;
using AwesomeAssertions;

namespace BanLedgerApiXUnitTests.UnitTests;

public class StatementServiceTests
{
    [Fact]
    public async Task GetAsync_WhenAccountMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = new StatementService(db.Context);

        var response = await service.GetAsync(
            Guid.NewGuid(),
            new StatementQuery(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow)));

        response.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_BuildsRunningBalanceWithOpeningAndDepositCounterparty()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        var account = await db.SeedAccountAsync(customer.Id, "1111111111");
        var counterparty = await db.SeedAccountAsync(customer.Id, "9999999999");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);

        db.Context.Transfers.AddRange(
            Deposit(account.Id, 100m, yesterday.ToDateTime(new TimeOnly(12, 0))),
            Deposit(account.Id, 500m, today.ToDateTime(new TimeOnly(10, 0))),
            TransferOut(account.Id, counterparty.Id, 200m, today.ToDateTime(new TimeOnly(11, 0))));
        await db.Context.SaveChangesAsync();

        var service = new StatementService(db.Context);

        var response = await service.GetAsync(account.Id, new StatementQuery(today, today));

        response.Should().NotBeNull();
        response!.OpeningBalance.Should().Be(100m);
        response.ClosingBalance.Should().Be(400m);
        response.Entries.Should().HaveCount(2);

        response.Entries[0].Direction.Should().Be(EntryDirectionEnum.Credit);
        response.Entries[0].Amount.Should().Be(500m);
        response.Entries[0].CounterpartyAccountNumber.Should().Be("Deposit");
        response.Entries[0].BalanceAfter.Should().Be(600m);

        response.Entries[1].Direction.Should().Be(EntryDirectionEnum.Debit);
        response.Entries[1].Amount.Should().Be(200m);
        response.Entries[1].CounterpartyAccountNumber.Should().Be("9999999999");
        response.Entries[1].BalanceAfter.Should().Be(400m);
    }

    private static Transfer Deposit(Guid destinationId, decimal amount, DateTime createdAt) => new()
    {
        SourceAccountId = null,
        DestinationAccountId = destinationId,
        Amount = amount,
        TransactionId = Guid.NewGuid(),
        Status = TransferStatusEnum.Completed,
        CreatedAt = createdAt
    };

    private static Transfer TransferOut(Guid sourceId, Guid destinationId, decimal amount, DateTime createdAt) => new()
    {
        SourceAccountId = sourceId,
        DestinationAccountId = destinationId,
        Amount = amount,
        TransactionId = Guid.NewGuid(),
        Status = TransferStatusEnum.Completed,
        CreatedAt = createdAt
    };
}
