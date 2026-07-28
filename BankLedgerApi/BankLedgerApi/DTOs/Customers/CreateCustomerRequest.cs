using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Customers;

public record CreateCustomerRequest(
    string Name,
    string DocumentNumber,
    DocumentTypeEnum DocumentType);
