using CatalogApi.Models;
using System.Collections.ObjectModel;

namespace CatalogApi.DTOs;

public class CategoryResponseDto:CategoryRequestDto,INamed
{
    public int CategoryId { get; set; }
    public ICollection<ProductResponseDto>? Products { get; set; } = new Collection<ProductResponseDto>();
}