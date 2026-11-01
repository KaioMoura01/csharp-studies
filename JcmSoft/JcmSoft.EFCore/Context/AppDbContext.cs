using JcmSoft.Domain.Entities;
using JcmSoft.EFCore.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JcmSoft.EFCore.Context;

public class AppDbContext:DbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeDetails> EmployeesDetails { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(AppConfig.GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}