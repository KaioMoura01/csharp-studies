using JcmSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JcmSoft.EFCore.Configurations;

public class ProjectConfiguration:IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
    {
        entity.HasIndex(p => p.Id);
        entity.Property(p => p.Budget).HasPrecision(18,2).IsRequired();
        entity.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
        entity.Property(p => p.Name).HasMaxLength(50).IsRequired();
        entity.Property(p => p.Description).HasMaxLength(150);
    }
}