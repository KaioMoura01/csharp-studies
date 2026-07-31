using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Domain.Models.Interfaces;

public interface IAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Number { get; set; }
    public AccountTypeEnum Type { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
}