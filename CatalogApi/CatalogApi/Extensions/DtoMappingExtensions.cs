using CatalogApi.DTOs;
using CatalogApi.Models;

namespace CatalogApi.Extensions;

public static class DtoMappingExtensions
{
    public static ProductResponseDto ToResponseDto(this Product p) => new()
    {
        ProductId = p.ProductId,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        ImageUrl = p.ImageUrl,
        Stock = p.Stock,
        CategoryId = p.CategoryId,
        CreatedAt = p.CreatedAt
    };

    public static CategoryResponseDto ToResponseDto(this Category c) => new()
    {
        CategoryId = c.CategoryId,
        Name = c.Name,
        ImageUrl = c.ImageUrl,
        Products = c.Products?.Select(p => p.ToResponseDto()).ToList()
    };

    public static IEnumerable<ProductResponseDto> ToResponseDto(this IEnumerable<Product> items)
        => items.Select(ToResponseDto);

    public static IEnumerable<CategoryResponseDto> ToResponseDto(this IEnumerable<Category> items)
        => items.Select(ToResponseDto);
}