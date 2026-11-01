using JcmSoft.Domain.Entities.Enums;

namespace JcmSoft.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Budget { get; set; }
    public DateOnly InitDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateOnly FinishDate { get; set; }
    public ProjectStatusEnum Status { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];

    public Guid ClientId { get; set; }
    public Client? Client { get; set; }
}