using KShop.UserApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace KShop.UserApi.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; }

    private static void UserProfilesModelCreating(ModelBuilder mb)
    {
        var entity = mb.Entity<UserProfile>();
        entity.HasKey(e => e.Id);
        entity.Property(e => e.KeycloakSubjectId).HasMaxLength(100).IsRequired();
        entity.HasIndex(e => e.KeycloakSubjectId).IsUnique();
        entity.Property(e => e.DisplayName).HasMaxLength(100);
        entity.Property(e => e.Email).HasMaxLength(150);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        UserProfilesModelCreating(modelBuilder);
    }
}
