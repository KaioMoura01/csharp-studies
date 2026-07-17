namespace PayrollProject.Entities;

public class HourlyEmployee(string name, double salaryPerHour, int workedHours):
    Employee(name, salaryPerHour)
{
    public int WorkedHours { get; set; } = workedHours;

    public override double CalculatePayment()
    {
        return base.CalculatePayment() * WorkedHours;
    }

    public override string ToString()
    {
        return string.Concat(base.ToString(), " Worked Hours: ", WorkedHours);
    }
}