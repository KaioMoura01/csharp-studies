using System.Globalization;
using SalesReportProject.Domain;

namespace SalesReportProject.Entities;

public class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    public Product(string name, double price, int quantity)
    {
        if (name.Length < 3)
        {
            throw new DomainException("Product name is too short.");
        }

        if (price <= 0)
        {
            throw new DomainException("Price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }
        
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    override public string ToString()
    {
        return $"{Name}, {Price.ToString("F2", CultureInfo.InvariantCulture)} x {Quantity}";
    }
}