using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Domain.Models;

public class TaxDocument
{
    public string Number { get; private set; }
        public DocumentTypeEnum Type { get; private set; }

        public TaxDocument(string number, DocumentTypeEnum type)
        {
            var digits = NormalizeDigits(number);
            var expected = type == DocumentTypeEnum.Cpf ? 11 : 14;
            if (digits.Length != expected)
                throw new ArgumentException($"{type} deve ter {expected} dígitos.");

            Number = digits;
            Type = type;
        }

        public static string NormalizeDigits(string number) =>
            new(number.Where(char.IsDigit).ToArray());
}