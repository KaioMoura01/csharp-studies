using System.Globalization;

namespace ProductRegistrationProject.Entities;

public class ImportedProduct(string name, double price, double importDuty):
    Product(name, price)
{
    private double ImportDuty { get; set; } = importDuty;
    protected override double Price => base.Price + ImportDuty;

    public override string ToString()
    {
        return string.Concat(
            base.ToString(),
            " Import Duty (included in price): ",
            ImportDuty.ToString("F2", CultureInfo.InvariantCulture)
            );
    }
}