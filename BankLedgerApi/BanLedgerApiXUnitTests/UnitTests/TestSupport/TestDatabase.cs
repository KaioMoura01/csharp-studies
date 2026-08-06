using BankLedgerApi.Application.Multitenancy;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Infrastructure.Persistence;
using BankLedgerApi.Infrastructure.Repositories;
using BankLedgerApi.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BanLedgerApiXUnitTests.UnitTests.TestSupport;

public sealed class FixedTenantContext(Guid tenantId) : ITenantContext
{
    public Guid? TenantId { get; } = tenantId;
}

public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public Guid TenantId { get; } = Guid.NewGuid();
    public ITenantContext TenantContext { get; }
    public AppDbContext Context { get; }
    public IPasswordHasher PasswordHasher { get; } = new PasswordHasherAdapter();
    public AccountRepository AccountRepository { get; }
    public CustomerRepository CustomerRepository { get; }
    public TransferRepository TransferRepository { get; }
    public EfUnitOfWork UnitOfWork { get; }

    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        TenantContext = new FixedTenantContext(TenantId);
        Context = new AppDbContext(options, TenantContext);
        Context.Database.EnsureCreated();

        AccountRepository = new AccountRepository(Context);
        CustomerRepository = new CustomerRepository(Context);
        TransferRepository = new TransferRepository(Context);
        UnitOfWork = new EfUnitOfWork(Context);
    }

    public async Task<Customer> SeedCustomerAsync(
        string name = "Test Customer",
        string document = "12345678901",
        DocumentTypeEnum type = DocumentTypeEnum.Cpf,
        string password = "1234")
    {
        var customer = new Customer
        {
            TenantId = TenantId,
            Name = name,
            TaxDocument = new TaxDocument(document, type),
            PasswordHash = PasswordHasher.HashPassword(password)
        };

        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();
        return customer;
    }

    public async Task<Account> SeedAccountAsync(
        Guid customerId,
        string number,
        decimal balance = 0m,
        bool active = true,
        AccountTypeEnum type = AccountTypeEnum.Checking,
        DateTime? createdAt = null)
    {
        var account = new Account
        {
            TenantId = TenantId,
            Name = $"Account {number}",
            Number = number,
            Type = type,
            CurrentBalance = balance,
            IsActive = active,
            CustomerId = customerId,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

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
