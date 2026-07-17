using System.Globalization;
using PayrollProject.Entities;
using PayrollProject.Misc;

var employees = new List<Employee>();

Console.WriteLine("=== Welcome to PayrollProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Add salaried employee");
    Console.WriteLine("2 - Add hourly employee");
    Console.WriteLine("3 - Add freelancer");
    Console.WriteLine("4 - List employees and payments");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var name = Utils.ReadString("Name: ");
                var salary = Utils.ReadPositiveDouble("Salary: ");

                employees.Add(new SalariedEmployee(name, salary));
                Console.WriteLine("Salaried employee added successfully!");
                break;
            }
            case 2:
            {
                var name = Utils.ReadString("Name: ");
                var salaryPerHour = Utils.ReadPositiveDouble("Salary per hour: ");
                var workedHours = Utils.ReadPositiveInt("Worked hours: ");

                employees.Add(new HourlyEmployee(name, salaryPerHour, workedHours));
                Console.WriteLine("Hourly employee added successfully!");
                break;
            }
            case 3:
            {
                var name = Utils.ReadString("Name: ");
                var baseSalary = Utils.ReadPositiveDouble("Payment per deliverable: ");
                var deliverables = Utils.ReadPositiveInt("Deliverables: ");

                employees.Add(new Freelancer(name, baseSalary, deliverables));
                Console.WriteLine("Freelancer added successfully!");
                break;
            }
            case 4:
                if (employees.Count == 0)
                {
                    Console.WriteLine("No employees registered yet.");
                    break;
                }

                foreach (var employee in employees)
                {
                    Console.WriteLine(employee);
                    Console.WriteLine(
                        $"  -> Payment: {employee.CalculatePayment().ToString("F2", CultureInfo.InvariantCulture)}");
                }
                break;
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}

return;
