using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Application.Dtos;

public class TodoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateTodoDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public DateTime? DueDate { get; set; }
}

public class UpdateTodoDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TodoPriority Priority { get; set; }
    public TodoStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
}
