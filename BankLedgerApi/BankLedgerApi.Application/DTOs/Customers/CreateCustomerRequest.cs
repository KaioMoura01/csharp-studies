using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Customers;

public record CreateCustomerRequest(
    string Name,
    string DocumentNumber,
    DocumentTypeEnum DocumentType,
    string Password);
