namespace MinimalCatalog.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Image { get; set; }
    public DateTime PurchasedAt { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}