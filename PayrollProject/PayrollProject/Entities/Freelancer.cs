namespace PayrollProject.Entities;

public class Freelancer(string name, double baseSalary, int deliverables):
    Employee(name, baseSalary)
{
    private int Deliverables { get; set; } = deliverables;

    public override double CalculatePayment()
    {
        return base.CalculatePayment() * Deliverables;
    }

    public override string ToString()
    {
        return string.Concat(base.ToString(), ", Deliverables: ", Deliverables);
    }
}