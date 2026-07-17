namespace SalesReportProject.Entities;

public class Seller(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;

    public override string ToString()
    {
        return $"#{Id} - {Name}";
    }
}