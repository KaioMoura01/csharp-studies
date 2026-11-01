using JcmSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JcmSoft.EFCore.Configurations;

public class ClientConfiguration:IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> entity)
    {
        entity.HasIndex(c => c.Id);
        entity.Property(c => c.Name).HasMaxLength(70).IsRequired();
        entity.Property(c => c.Phone).HasMaxLength(11).IsRequired();
        entity.Property(c => c.Email).HasMaxLength(30).IsRequired();

        entity.HasMany(c => c.Projects).WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId).OnDelete(DeleteBehavior.Cascade);
    }
}