using SalesReportProject.Entities;
using SalesReportProject.Misc;

namespace SalesReportProject.Services;

public class SalesReportService(List<Sell> sales)
{
    public static double Total(Sell sale) => sale.ProductsSold.Sum(p => p.Price * p.Quantity);

    public void TotalPerSeller(TextWriter w)
    {
        var report = sales
            .GroupBy(s => s.Seller)
            .Select(g => new { Seller = g.Key, Total = g.Sum(Total) })
            .OrderByDescending(x => x.Total);

        w.WriteLine("=== Total per seller ===");
        foreach (var row in report)
        {
            w.WriteLine($"{row.Seller.Name}: {Utils.Money(row.Total)}");
        }
    }

    public void MonthlyAverage(TextWriter w)
    {
        var byMonth = sales
            .GroupBy(s => new { s.Date.Year, s.Date.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(Total) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToList();

        w.WriteLine("=== Monthly total ===");
        foreach (var row in byMonth)
        {
            w.WriteLine($"{row.Year}-{row.Month:D2}: {Utils.Money(row.Total)}");
        }

        var average = byMonth.Average(x => x.Total);
        w.WriteLine($"Average per month: {Utils.Money(average)}");
    }

    public void TopProducts(TextWriter w, int take = 3)
    {
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
            .Take(take);

        w.WriteLine($"=== Top {take} products by revenue ===");
        var rank = 1;
        foreach (var row in top)
        {
            w.WriteLine($"{rank++}) {row.Name} - revenue {Utils.Money(row.Revenue)} ({row.Quantity} units)");
        }
    }

    public List<Sell> InPeriod(DateTime start, DateTime end) =>
        sales
            .Where(s => s.Date >= start && s.Date <= end)
            .OrderBy(s => s.Date)
            .ToList();
}
