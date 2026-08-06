namespace KShop.ProductApi.Application.DTOs.Products;

public record CreateProductRequest(
    string? Name,
    string? Description,
    string? ImageUrl,
    decimal Price,
    long Stock,
    Guid CategoryId);
