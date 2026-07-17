using System.Globalization;
using System.Text;

namespace SalesReportProject.Entities;

public class Sell(int id, DateTime date, Seller seller)
{
    public int Id { get; set; } = id;
    public DateTime Date { get; set; } = date;
    public Seller Seller { get; set; } = seller;
    public List<Product> ProductsSold { get; set; } = [];
    
    public void AddProductToSell(Product product)
    {
        ProductsSold.Add(product);
    }
    
    public void RemoveProductFromSell(Product product)
    {
        ProductsSold.Remove(product);
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"#{Id}");
        sb.AppendLine($"Date: {Date}");
        sb.AppendLine($"Seller: {Seller}");

        var sum = ProductsSold.Sum(x => x.Price *  x.Quantity);
        
        foreach (var product in ProductsSold)
        {
            sb.AppendLine(product.ToString());
        }
        
        sb.AppendLine($"Products Sold: {ProductsSold.Count}");
        sb.AppendLine($"Subtotal: {sum.ToString("F2", CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }
}