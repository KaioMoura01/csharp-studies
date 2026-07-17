using System.Globalization;
using SalesReportProject.Domain;
using SalesReportProject.Entities;
using SalesReportProject.Misc;

var sales = new List<Sell>();
var sellers = new List<Seller>();
var nextSaleId = 1;
var nextSellerId = 1;

Func<Sell, double> saleTotal = sale => sale.ProductsSold.Sum(p => p.Price * p.Quantity);
Func<double, string> money = value => value.ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine("=== Welcome to SalesReportProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Register sale");
    Console.WriteLine("2 - Report: total per seller");
    Console.WriteLine("3 - Report: monthly average");
    Console.WriteLine("4 - Report: top 3 products");
    Console.WriteLine("5 - Report: filter by period");
    Console.WriteLine("6 - List all sales");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
                RegisterSale();
                break;
            case 2:
                ReportTotalPerSeller();
                break;
            case 3:
                ReportMonthlyAverage();
                break;
            case 4:
                ReportTopProducts();
                break;
            case 5:
                ReportByPeriod();
                break;
            case 6:
                ListAllSales();
                break;
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (DomainException e)
    {
        Console.WriteLine($"Validation error: {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}

return;

void RegisterSale()
{
    var sellerName = Utils.ReadString("Seller name: ");
    var seller = sellers.FirstOrDefault(s => s.Name.Equals(sellerName, StringComparison.OrdinalIgnoreCase));
    if (seller is null)
    {
        seller = new Seller(nextSellerId++, sellerName);
        sellers.Add(seller);
    }

    var date = Utils.ReadDate("Sale date (dd/MM/yyyy): ");
    var sale = new Sell(nextSaleId++, date, seller);

    var productCount = Utils.ReadInt("How many products in this sale? ");
    while (productCount < 1)
    {
        Console.WriteLine("A sale must have at least one product.");
        productCount = Utils.ReadInt("How many products in this sale? ");
    }

    for (var i = 0; i < productCount; i++)
    {
        Console.WriteLine($"-- Product {i + 1} --");
        var name = Utils.ReadString("Product name: ");
        var price = Utils.ReadDouble("Value (price): ");
        var quantity = Utils.ReadInt("Quantity: ");

        sale.AddProductToSell(new Product(name, price, quantity));
    }

    sales.Add(sale);
    Console.WriteLine($"Sale registered successfully! Total: {money(saleTotal(sale))}");
}

void ReportTotalPerSeller()
{
    if (NoSales()) return;

    var report = sales
        .GroupBy(s => s.Seller)
        .Select(g => new { Seller = g.Key, Total = g.Sum(saleTotal) })
        .OrderByDescending(x => x.Total);

    Console.WriteLine("=== Total per seller ===");
    foreach (var row in report)
    {
        Console.WriteLine($"{row.Seller.Name}: {money(row.Total)}");
    }
}

void ReportMonthlyAverage()
{
    if (NoSales()) return;

    var byMonth = sales
        .GroupBy(s => new { s.Date.Year, s.Date.Month })
        .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(saleTotal) })
        .OrderBy(x => x.Year).ThenBy(x => x.Month)
        .ToList();

    Console.WriteLine("=== Monthly total ===");
    foreach (var row in byMonth)
    {
        Console.WriteLine($"{row.Year}-{row.Month:D2}: {money(row.Total)}");
    }

    var average = byMonth.Average(x => x.Total);
    Console.WriteLine($"Average per month: {money(average)}");
}

void ReportTopProducts()
{
    if (NoSales()) return;

    var top = sales
        .SelectMany(s => s.ProductsSold)
        .GroupBy(p => p.Name)
        .Select(g => new
        {
            Name = g.Key,
            Quantity = g.Sum(p => p.Quantity),
            Revenue = g.Sum(p => p.Price * p.Quantity)
        })
        .OrderByDescending(x => x.Revenue)
        .Take(3);

    Console.WriteLine("=== Top 3 products by revenue ===");
    var rank = 1;
    foreach (var row in top)
    {
        Console.WriteLine($"{rank++}) {row.Name} - revenue {money(row.Revenue)} ({row.Quantity} units)");
    }
}

void ReportByPeriod()
{
    if (NoSales()) return;

    var start = Utils.ReadDate("Start date (dd/MM/yyyy): ");
    var end = Utils.ReadDate("End date (dd/MM/yyyy): ");

    var filtered = sales
        .Where(s => s.Date >= start && s.Date <= end)
        .OrderBy(s => s.Date)
        .ToList();

    Console.WriteLine($"=== Sales between {start:dd/MM/yyyy} and {end:dd/MM/yyyy} ===");
    if (filtered.Count == 0)
    {
        Console.WriteLine("No sales in this period.");
        return;
    }

    foreach (var sale in filtered)
    {
        Console.WriteLine($"#{sale.Id} | {sale.Date:dd/MM/yyyy} | {sale.Seller.Name} | {money(saleTotal(sale))}");
    }

    Console.WriteLine($"Period total: {money(filtered.Sum(saleTotal))}");
}

void ListAllSales()
{
    if (NoSales()) return;

    foreach (var sale in sales.OrderBy(s => s.Date))
    {
        Console.WriteLine(sale);
        Console.WriteLine();
    }
}

bool NoSales()
{
    if (sales.Count != 0) return false;
    Console.WriteLine("No sales registered yet.");
    return true;
}
