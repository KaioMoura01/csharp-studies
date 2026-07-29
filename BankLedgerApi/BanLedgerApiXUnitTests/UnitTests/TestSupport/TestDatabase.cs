using BankLedgerApi.Context;
using BankLedgerApi.Enums;
using BankLedgerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BanLedgerApiXUnitTests.UnitTests.TestSupport;

public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Context { get; }
    public IPasswordHasher<Account> PasswordHasher { get; } = new PasswordHasher<Account>();

    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public async Task<Customer> SeedCustomerAsync(
        string name = "Test Customer",
        string document = "12345678901",
        DocumentTypeEnum type = DocumentTypeEnum.Cpf)
    {
        var customer = new Customer
        {
            Name = name,
            TaxDocument = new TaxDocument(document, type)
        };

        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();
        return customer;
    }

    public async Task<Account> SeedAccountAsync(
        Guid customerId,
        string number,
        string password = "1234",
        decimal balance = 0m,
        bool active = true,
        AccountTypeEnum type = AccountTypeEnum.Checking)
    {
        var account = new Account
        {
            Name = $"Account {number}",
            Number = number,
            Type = type,
            PasswordHash = string.Empty,
            CurrentBalance = balance,
            IsActive = active,
            CustomerId = customerId
        };

        account.PasswordHash = PasswordHasher.HashPassword(account, password);

        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();
        return account;
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
