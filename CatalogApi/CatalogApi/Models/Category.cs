using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CatalogApi.Models;

public class Category:INamed
{
    [BindNever]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    [BindNever]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public ICollection<Product>? Products { get; set; } = new Collection<Product>();
}