using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models.Interfaces;

namespace BankLedgerApi.Domain.Models;

public class Account:IAccount
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required string Number { get; set; }
    public AccountTypeEnum Type { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}