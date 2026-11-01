using Microsoft.EntityFrameworkCore;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        TodosModelCreating(modelBuilder);
    }

    private static void TodosModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).HasMaxLength(2000);
            builder.Property(t => t.OwnerId).IsRequired();
            builder.HasIndex(t => t.OwnerId);
            builder.Property(t => t.Priority).HasConversion<string>();
            builder.Property(t => t.Status).HasConversion<string>();
        });
    }
}
