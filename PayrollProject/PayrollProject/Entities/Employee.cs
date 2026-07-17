using System.Globalization;

namespace PayrollProject.Entities;

public abstract class Employee(string name, double baseSalary)
{
    private string Name { get; set; } = name;
    private double BaseSalary { get; set; } = baseSalary;
    
    public virtual double CalculatePayment()
    {
        return BaseSalary;
    }

    public override string ToString()
    {
        return string.Concat(
            "Name: ",
            Name,
            ", Base salary: ",
            BaseSalary.ToString("F2", CultureInfo.InvariantCulture)
            );
    }
}