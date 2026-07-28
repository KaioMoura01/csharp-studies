using BankLedgerApi.Enums;
using BankLedgerApi.Models.Interfaces;

namespace BankLedgerApi.Models;

public class Account:IAccount
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Number { get; set; }
    public AccountTypeEnum Type { get; set; }
    public required string PasswordHash { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}