using JcmSoft.Domain.Entities;
using JcmSoft.Domain.Entities.Enums;
using JcmSoft.EFCore.Context;

namespace JcmSoft.EFCore.Seed;

public static class DatabaseSeeder
{
    private const string SeedFolderName = "SeedData";

    public static void Seed(AppDbContext context, string? seedDataPath = null)
    {
        if (context.Departments.Any())
        {
            Console.WriteLine("Database already have data, ignoring seed...");
            return;
        }

        var folder = seedDataPath ?? Path.Combine(AppContext.BaseDirectory, SeedFolderName);

        var departments = SeedDepartments(context, folder);
        var clients = SeedClients(context, folder);
        var employees = SeedEmployees(context, folder, departments);
        SeedEmployeeDetails(context, folder, employees);
        var projects = SeedProjects(context, folder, clients);
        LinkProjectsAndEmployees(context, folder, projects, employees);

        context.SaveChanges();

        Console.WriteLine(
            $"Seed concluded: {departments.Count} departments, {employees.Count} employees, " +
            $"{clients.Count} clients and {projects.Count} projects.");
    }

    private static Dictionary<string, Department> SeedDepartments(AppDbContext context, string folder)
    {
        var departments = new Dictionary<string, Department>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in CsvFile.Read(Path.Combine(folder, "departments.csv")))
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = row.GetString("name"),
                Description = row.GetOptionalString("description")
            };
            departments.Add(row.GetString("code"), department);
        }

        context.Departments.AddRange(departments.Values);
        return departments;
    }

    private static Dictionary<string, Client> SeedClients(AppDbContext context, string folder)
    {
        var clients = new Dictionary<string, Client>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in CsvFile.Read(Path.Combine(folder, "clients.csv")))
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                Name = row.GetString("name"),
                Email = row.GetString("email"),
                Phone = row.GetString("phone")
            };
            clients.Add(row.GetString("code"), client);
        }

        context.Clients.AddRange(clients.Values);
        return clients;
    }

    private static Dictionary<string, Employee> SeedEmployees(
        AppDbContext context,
        string folder,
        Dictionary<string, Department> departments)
    {
        var employees = new Dictionary<string, Employee>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in CsvFile.Read(Path.Combine(folder, "employees.csv")))
        {
            var departmentCode = row.GetString("department_code");
            if (!departments.TryGetValue(departmentCode, out var department))
                throw new InvalidDataException($"employees.csv: departamento '{departmentCode}' doesn't exists in departments.csv.");

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = row.GetString("name"),
                Role = row.GetString("role"),
                Salary = row.GetDecimal("salary"),
                InitDate = row.GetDate("init_date"),
                Department = department
            };
            employees.Add(row.GetString("code"), employee);
        }

        context.Employees.AddRange(employees.Values);
        return employees;
    }

    private static void SeedEmployeeDetails(
        AppDbContext context,
        string folder,
        Dictionary<string, Employee> employees)
    {
        var details = new List<EmployeeDetails>();

        foreach (var row in CsvFile.Read(Path.Combine(folder, "employee_details.csv")))
        {
            var employeeCode = row.GetString("employee_code");
            if (!employees.TryGetValue(employeeCode, out var employee))
                throw new InvalidDataException($"employee_details.csv: employee '{employeeCode}' doesn't exists in employees.csv.");

            details.Add(new EmployeeDetails
            {
                Id = Guid.NewGuid(),
                Birthdate = row.GetDate("birthdate"),
                Genre = row.GetEnum<GenreEnum>("genre"),
                EducationBackground = row.GetEnum<EducationalBackgroundEnum>("education_background"),
                MaritalStatus = row.GetEnum<MaritalStatusEnum>("marital_status"),
                Phone = row.GetOptionalString("phone"),
                Photo = row.GetOptionalString("photo"),
                TaxDocument = row.GetString("tax_document"),
                Nationality = row.GetOptionalString("nationality"),
                Address = row.GetString("address"),
                Employee = employee
            });
        }

        context.EmployeesDetails.AddRange(details);
    }

    private static Dictionary<string, Project> SeedProjects(
        AppDbContext context,
        string folder,
        Dictionary<string, Client> clients)
    {
        var projects = new Dictionary<string, Project>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in CsvFile.Read(Path.Combine(folder, "projects.csv")))
        {
            var clientCode = row.GetString("client_code");
            if (!clients.TryGetValue(clientCode, out var client))
                throw new InvalidDataException($"projects.csv: client '{clientCode}' doesn't exists in clients.csv.");

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = row.GetString("name"),
                Description = row.GetOptionalString("description"),
                Budget = row.GetDecimal("budget"),
                InitDate = row.GetDate("init_date"),
                FinishDate = row.GetDate("finish_date"),
                Status = row.GetEnum<ProjectStatusEnum>("status"),
                Client = client
            };
            projects.Add(row.GetString("code"), project);
        }

        context.Projects.AddRange(projects.Values);
        return projects;
    }

    private static void LinkProjectsAndEmployees(
        AppDbContext context,
        string folder,
        Dictionary<string, Project> projects,
        Dictionary<string, Employee> employees)
    {
        foreach (var row in CsvFile.Read(Path.Combine(folder, "project_employees.csv")))
        {
            var projectCode = row.GetString("project_code");
            if (!projects.TryGetValue(projectCode, out var project))
                throw new InvalidDataException($"project_employees.csv: project '{projectCode}' doesn't exists in projects.csv.");

            var employeeCode = row.GetString("employee_code");
            if (!employees.TryGetValue(employeeCode, out var employee))
                throw new InvalidDataException($"project_employees.csv: employee '{employeeCode}' doesn't exists in employees.csv.");

            project.Employees.Add(employee);
        }
    }
}
