using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Context;

public class CatalogApiContext(DbContextOptions<CatalogApiContext> options) 
    : DbContext(options)
{
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }
}