using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Domain.Models;

public class Transfer
{
    public Guid Id { get; set; }
    public Guid? SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public Guid TransactionId { get; set; }
    public TransferStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    public Guid? ReversedTransferId { get; set; }
}