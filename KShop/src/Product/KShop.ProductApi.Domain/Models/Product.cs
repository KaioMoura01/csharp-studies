namespace KShop.ProductApi.Domain.Models;

public class Product
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public long Stock { get; set; }

    public Category? Category { get; set; }
    public Guid CategoryId { get; set; }
}