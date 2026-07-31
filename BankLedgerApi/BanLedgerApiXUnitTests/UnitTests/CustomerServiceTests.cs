using BanLedgerApiXUnitTests.UnitTests.TestSupport;
using BankLedgerApi.Application.DTOs.Customers;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Application.Services;
using AwesomeAssertions;

namespace BanLedgerApiXUnitTests.UnitTests;

public class CustomerServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidData_PersistsCustomerWithDocument()
    {
        using var db = new TestDatabase();
        var service = new CustomerService(db.CustomerRepository, db.UnitOfWork, db.PasswordHasher);

        var response = await service.CreateAsync(
            new CreateCustomerRequest("Kaio", "12345678901", DocumentTypeEnum.Cpf, "1234"));

        response.Name.Should().Be("Kaio");
        response.Document.Number.Should().Be("12345678901");
        response.Document.Type.Should().Be(DocumentTypeEnum.Cpf);
        response.Accounts.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDocument_Throws()
    {
        using var db = new TestDatabase();
        var service = new CustomerService(db.CustomerRepository, db.UnitOfWork, db.PasswordHasher);

        var act = () => service.CreateAsync(
            new CreateCustomerRequest("Kaio", "123", DocumentTypeEnum.Cpf, "1234"));

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsCustomerWithAccounts()
    {
        using var db = new TestDatabase();
        var customer = await db.SeedCustomerAsync();
        await db.SeedAccountAsync(customer.Id, "1000000001");
        var service = new CustomerService(db.CustomerRepository, db.UnitOfWork, db.PasswordHasher);

        var response = await service.GetByIdAsync(customer.Id);

        response.Should().NotBeNull();
        response!.Accounts.Should().ContainSingle(a => a.Number == "1000000001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        using var db = new TestDatabase();
        var service = new CustomerService(db.CustomerRepository, db.UnitOfWork, db.PasswordHasher);

        var response = await service.GetByIdAsync(Guid.NewGuid());

        response.Should().BeNull();
    }
}
