using System.Globalization;

namespace ProductRegistrationProject.Entities;

public abstract class Product(string name, double price)
{
    private string Name { get; set; } = name;
    protected virtual double Price { get; set; } = price;
    
    public override string ToString()
    {
        return $"{Name}: {Price.ToString("F2", CultureInfo.InvariantCulture)}";
    }
}