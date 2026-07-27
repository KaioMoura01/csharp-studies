using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<Loan> Loan { get; set; }
    public DbSet<Book> Book { get; set; }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Name);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Name).HasMaxLength(120);
            e.Property(u => u.Email).HasMaxLength(160);
            e.Property(u => u.PasswordHash).HasMaxLength(256);
        });
    }

    private static void ConfigureBook(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasIndex(b => b.Name);
        
        modelBuilder.Entity<Book>(e =>
        {
            e.Property(b => b.Name).HasMaxLength(60);
            e.Property(b => b.Publisher).HasMaxLength(30);
            e.Property(b => b.Description).HasMaxLength(250);
        });
    }

    private static void ConfigureLoan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany()
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureBook(modelBuilder);
        ConfigureLoan(modelBuilder);
    }
}