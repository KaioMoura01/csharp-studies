using KShop.ProductApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KShop.ProductApi.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    private static void ProductsModelCreating(ModelBuilder mb)
    {
        var entity = mb.Entity<Product>();
        entity.HasKey(e => e.Id);
        entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
        entity.Property(c => c.Description).HasMaxLength(150).IsRequired();
        entity.Property(c => c.ImageUrl).HasMaxLength(250);
        entity.Property(c => c.Price).HasPrecision(12, 2).IsRequired();
    }

    private static void CategoriesModelCreating(ModelBuilder mb)
    {
        var entity = mb.Entity<Category>();
        entity.HasKey(e => e.Id);
        entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
        
        entity.HasMany(c=>c.Products).WithOne(c=>c.Category)
            .IsRequired().OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ProductsModelCreating(modelBuilder);
        CategoriesModelCreating(modelBuilder);
    }
}