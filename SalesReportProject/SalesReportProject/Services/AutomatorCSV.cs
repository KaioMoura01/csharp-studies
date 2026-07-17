using System.Globalization;
using SalesReportProject.Domain;
using SalesReportProject.Entities;

namespace SalesReportProject.Services;

public static class AutomatorCSV
{
    public static List<Sell> ReadSales(string path)
    {
        if (!File.Exists(path))
        {
            throw new DomainException($"CSV file not found: {path}");
        }

        var sellersByName = new Dictionary<string, Seller>(StringComparer.OrdinalIgnoreCase);
        var salesById = new Dictionary<int, Sell>();
        var nextSellerId = 1;

        var rows = File.ReadAllLines(path)
            .Select((line, index) => (line, number: index + 1))
            .Skip(1)
            .Where(r => !string.IsNullOrWhiteSpace(r.line));

        foreach (var (line, number) in rows)
        {
            var cols = line.Split(',');
            if (cols.Length < 6)
            {
                throw new DomainException($"Invalid CSV line {number}: expected 6 columns.");
            }

            var saleId = int.Parse(cols[0], CultureInfo.InvariantCulture);
            var date = DateTime.ParseExact(cols[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var sellerName = cols[2].Trim();
            var productName = cols[3].Trim();
            var price = double.Parse(cols[4], NumberStyles.Any, CultureInfo.InvariantCulture);
            var quantity = int.Parse(cols[5], CultureInfo.InvariantCulture);

            if (!sellersByName.TryGetValue(sellerName, out var seller))
            {
                seller = new Seller(nextSellerId++, sellerName);
                sellersByName[sellerName] = seller;
            }

            if (!salesById.TryGetValue(saleId, out var sale))
            {
                sale = new Sell(saleId, date, seller);
                salesById[saleId] = sale;
            }

            sale.AddProductToSell(new Product(productName, price, quantity));
        }

        return salesById.Values.OrderBy(s => s.Id).ToList();
    }

    public static void WriteReport(string path, string content)
    {
        File.WriteAllText(path, content);
    }
}
