namespace BankLedgerApi.Domain.Enums;

public enum TransferStatusEnum:int
{
    Pending,
    Completed,
    Failed,
    Reversed
}