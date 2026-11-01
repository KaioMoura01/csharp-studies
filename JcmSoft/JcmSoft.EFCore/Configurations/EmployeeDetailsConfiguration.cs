using JcmSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JcmSoft.EFCore.Configurations;

public class EmployeeDetailsConfiguration:IEntityTypeConfiguration<EmployeeDetails>
{
    public void Configure(EntityTypeBuilder<EmployeeDetails> entity)
    {
        entity.HasIndex(e => e.Id);
        entity.Property(e => e.TaxDocument).HasMaxLength(14).IsRequired();
        entity.Property(e => e.Address).HasMaxLength(250).IsRequired();
        entity.Property(e => e.Nationality).HasMaxLength(50);
        entity.Property(e => e.Photo).HasMaxLength(250);
        entity.Property(e => e.Phone).HasMaxLength(11);

    }
}