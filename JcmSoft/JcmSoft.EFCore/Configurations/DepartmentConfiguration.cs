using JcmSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JcmSoft.EFCore.Configurations;

public class DepartmentConfiguration:IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> entity)
    {
        
        entity.HasIndex(d => d.Id);
        entity.Property(d => d.Name).HasMaxLength(50).IsRequired();
        entity.Property(d => d.Description).HasMaxLength(150);
        
    }

}