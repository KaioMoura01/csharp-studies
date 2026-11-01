using JcmSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JcmSoft.EFCore.Configurations;

public class EmployeeConfiguration: IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entity)
    {
        entity.HasIndex(e=> e.Id);
        entity.HasIndex(e=> e.Name);
        entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Role).HasMaxLength(70).IsRequired();
        entity.Property(e => e.Salary).HasPrecision(10, 2);

        entity.HasOne(e => e.Department).WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.EmployeeDetails).WithOne(e=> e.Employee)
            .HasForeignKey<EmployeeDetails>(e=>e.EmployeeId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.Projects).WithMany(p => p.Employees)
            .UsingEntity(j => j.ToTable("EmployeesProjects"));
    }
}