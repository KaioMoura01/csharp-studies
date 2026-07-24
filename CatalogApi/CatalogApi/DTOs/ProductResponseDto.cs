using CatalogApi.Models;

namespace CatalogApi.DTOs;

public class ProductResponseDto:ProductRequestDto,INamed
{
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryResponseDto? Category { get; set; }
}