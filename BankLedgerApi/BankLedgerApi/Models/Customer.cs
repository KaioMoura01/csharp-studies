namespace BankLedgerApi.Models;

public class Customer
{
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public TaxDocument TaxDocument { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = [];
}