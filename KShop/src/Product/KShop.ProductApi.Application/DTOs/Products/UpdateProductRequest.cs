namespace KShop.ProductApi.Application.DTOs.Products;

public record UpdateProductRequest(
    Guid Id,
    string? Name,
    string? Description,
    string? ImageUrl,
    decimal Price,
    long Stock,
    Guid CategoryId);
