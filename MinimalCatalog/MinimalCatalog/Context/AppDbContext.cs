using Microsoft.EntityFrameworkCore;
using MinimalCatalog.Models;

namespace MinimalCatalog.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }

    private static void ConfigureCategory(ModelBuilder mb)
    {
        var categoryEntity = mb.Entity<Category>();
        
        categoryEntity.HasKey(c => c.CategoryId);
        categoryEntity.Property(c => c.Name)
            .HasMaxLength(100).IsRequired();
        categoryEntity.Property(c => c.Description)
            .HasMaxLength(150).IsRequired();
    }
    
    private static void ConfigureProduct(ModelBuilder mb)
    {
        var productEntity = mb.Entity<Product>();
        
        productEntity.HasKey(c => c.ProductId);
        productEntity.Property(c => c.Name)
            .HasMaxLength(100).IsRequired();
        productEntity.Property(c => c.Description)
            .HasMaxLength(150).IsRequired();
        productEntity.Property(c => c.Image)
            .HasMaxLength(150).IsRequired();
        productEntity.Property(c => c.Price).HasPrecision(14, 2);
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        ConfigureCategory(mb);
        ConfigureProduct(mb);
        
        mb.Entity<Product>().HasOne<Category>(c => c.Category)
            .WithMany().HasForeignKey(c=>c.CategoryId);
    }
}