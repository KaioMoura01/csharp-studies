using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CatalogApi.Models;

public class Product:INamed
{
    [BindNever]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }
    [BindNever]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CategoryId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Category? Category { get; set; }
}