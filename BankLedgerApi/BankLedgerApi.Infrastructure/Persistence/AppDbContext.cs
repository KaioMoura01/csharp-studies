using BankLedgerApi.Application.Multitenancy;
using BankLedgerApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTenant(modelBuilder);
        ConfigureCustomer(modelBuilder);
        ConfigureAccount(modelBuilder);
        ConfigureTransfer(modelBuilder);
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(t => t.Slug).IsUnique();

            entity.Property(t => t.IsActive)
                .HasDefaultValue(true);

            entity.Property(t => t.CreatedAt)
                .IsRequired();
        });
    }

    private void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.TenantId)
                .IsRequired();

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.PasswordHash)
                .IsRequired();

            entity.OwnsOne(c => c.TaxDocument, document =>
            {
                document.Property(d => d.Number)
                    .HasColumnName("DocumentNumber")
                    .HasMaxLength(14)
                    .IsRequired();

                document.Property(d => d.Type)
                    .HasColumnName("DocumentType")
                    .HasConversion<string>()
                    .HasMaxLength(10);
            });

            entity.Navigation(c => c.TaxDocument).IsRequired();

            entity.HasMany(c => c.Accounts)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(c => c.TenantId == tenantContext.TenantId);
        });
    }

    private void ConfigureAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.TenantId)
                .IsRequired();

            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(a => a.Number)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(a => new { a.TenantId, a.Number }).IsUnique();

            entity.Property(a => a.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(a => a.CurrentBalance)
                .HasPrecision(18, 2);

            entity.Property(a => a.IsActive)
                .HasDefaultValue(true);

            entity.Property(a => a.CreatedAt)
                .IsRequired();

            entity.HasQueryFilter(a => a.TenantId == tenantContext.TenantId);
        });
    }

    private void ConfigureTransfer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.TenantId)
                .IsRequired();

            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.HasIndex(t => t.TransactionId);
            entity.HasIndex(t => t.CreatedAt);
            entity.HasIndex(t => t.SourceAccountId);
            entity.HasIndex(t => t.DestinationAccountId);
            entity.HasIndex(t => t.ReversedTransferId);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.SourceAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.DestinationAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(t => t.TenantId == tenantContext.TenantId);
        });
    }
}
