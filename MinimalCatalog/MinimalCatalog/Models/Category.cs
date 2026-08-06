using Mapster;

namespace MinimalCatalog.Models;

[AdaptTo("[name]Dto"), GenerateMapper]
public class Category
{
    public Guid CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}