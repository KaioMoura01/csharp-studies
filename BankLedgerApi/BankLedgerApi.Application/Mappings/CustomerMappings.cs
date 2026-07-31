using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Application.DTOs.Customers;
using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Application.Mappings;

public static class CustomerMappings
{
    public static CustomerDetailsResponse ToDetails(this Customer customer) =>
        new(customer.Id,
            customer.Name,
            new FiscalDocumentDto(customer.TaxDocument.Number, customer.TaxDocument.Type),
            customer.Accounts.Select(account => account.ToSummary()).ToList());
}
