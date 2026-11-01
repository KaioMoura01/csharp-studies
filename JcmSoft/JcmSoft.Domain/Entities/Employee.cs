namespace JcmSoft.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public decimal Salary { get; set; }
    public DateOnly InitDate { get; set; }

    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public EmployeeDetails? EmployeeDetails { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}