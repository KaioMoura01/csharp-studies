using System.Globalization;
using SalesReportProject.Domain;
using SalesReportProject.Entities;
using SalesReportProject.Misc;
using SalesReportProject.Services;

var sales = new List<Sell>();
var sellers = new List<Seller>();
var nextSaleId = 1;
var nextSellerId = 1;

var reports = new SalesReportService(sales);

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
    Console.WriteLine("7 - Import sales from CSV");
    Console.WriteLine("8 - Export full report to file");
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
                if (NoSales()) break;
                reports.TotalPerSeller(Console.Out);
                break;
            case 3:
                if (NoSales()) break;
                reports.MonthlyAverage(Console.Out);
                break;
            case 4:
                if (NoSales()) break;
                reports.TopProducts(Console.Out);
                break;
            case 5:
                if (NoSales()) break;
                ReportByPeriod();
                break;
            case 6:
                if (NoSales()) break;
                ListAllSales();
                break;
            case 7:
                ImportFromCsv();
                break;
            case 8:
                if (NoSales()) break;
                ExportReportToFile();
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
    Console.WriteLine($"Sale registered successfully! Total: {Utils.Money(SalesReportService.Total(sale))}");
}

void ReportByPeriod()
{
    var start = Utils.ReadDate("Start date (dd/MM/yyyy): ");
    var end = Utils.ReadDate("End date (dd/MM/yyyy): ");

    var filtered = reports.InPeriod(start, end);

    Console.WriteLine($"=== Sales between {start:dd/MM/yyyy} and {end:dd/MM/yyyy} ===");
    if (filtered.Count == 0)
    {
        Console.WriteLine("No sales in this period.");
        return;
    }

    foreach (var sale in filtered)
    {
        Console.WriteLine($"#{sale.Id} | {sale.Date:dd/MM/yyyy} | {sale.Seller.Name} | {Utils.Money(SalesReportService.Total(sale))}");
    }

    Console.WriteLine($"Period total: {Utils.Money(filtered.Sum(SalesReportService.Total))}");
}

void ListAllSales()
{
    foreach (var sale in sales.OrderBy(s => s.Date))
    {
        Console.WriteLine(sale);
        Console.WriteLine();
    }
}

void ImportFromCsv()
{
    var defaultPath = Path.Combine(AppContext.BaseDirectory, "sales.csv");
    var path = Utils.ReadPath("CSV file path", defaultPath);

    var imported = AutomatorCSV.ReadSales(path);

    sales.Clear();
    sales.AddRange(imported);
    sellers.Clear();
    sellers.AddRange(imported.Select(s => s.Seller).DistinctBy(s => s.Id));
    nextSaleId = sales.Count == 0 ? 1 : sales.Max(s => s.Id) + 1;
    nextSellerId = sellers.Count == 0 ? 1 : sellers.Max(s => s.Id) + 1;

    Console.WriteLine($"Imported {sales.Count} sale(s) from CSV.");
}

void ExportReportToFile()
{
    var defaultPath = Path.Combine(AppContext.BaseDirectory, "sales-report.txt");
    var path = Utils.ReadPath("Output report path", defaultPath);

    using var writer = new StringWriter();
    writer.WriteLine("=== SALES REPORT ===");
    writer.WriteLine($"Generated at: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}");
    writer.WriteLine($"Total sales: {sales.Count}");
    writer.WriteLine();
    reports.TotalPerSeller(writer);
    writer.WriteLine();
    reports.MonthlyAverage(writer);
    writer.WriteLine();
    reports.TopProducts(writer);

    AutomatorCSV.WriteReport(path, writer.ToString());
    Console.WriteLine($"Report written to: {path}");
}

bool NoSales()
{
    if (sales.Count != 0) return false;
    Console.WriteLine("No sales registered yet.");
    return true;
}
