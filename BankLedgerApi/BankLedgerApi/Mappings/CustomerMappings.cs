using BankLedgerApi.DTOs.Common;
using BankLedgerApi.DTOs.Customers;
using BankLedgerApi.Models;

namespace BankLedgerApi.Mappings;

public static class CustomerMappings
{
    public static CustomerDetailsResponse ToDetails(this Customer customer) =>
        new(customer.Id,
            customer.Name,
            new FiscalDocumentDto(customer.TaxDocument.Number, customer.TaxDocument.Type),
            customer.Accounts.Select(account => account.ToSummary()).ToList());
}
