namespace TodoForSso.TodoApi.Domain;

public class TodoItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public TodoStatus Status { get; set; } = TodoStatus.Pending;

    /// <summary>Claim "sub" do Keycloak — dono da tarefa.</summary>
    public required string OwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    public void Complete()
    {
        if (Status == TodoStatus.Done)
        {
            throw new DomainException("Tarefa já está concluída.");
        }

        Status = TodoStatus.Done;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        Status = TodoStatus.Pending;
        CompletedAt = null;
    }
}
