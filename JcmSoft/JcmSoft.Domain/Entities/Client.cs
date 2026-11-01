namespace JcmSoft.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}