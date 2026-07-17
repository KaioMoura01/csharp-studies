using System.Text.Json;
using System.Text.Json.Serialization;
using ToDoProject.Entities;
using ToDoProject.Entities.Enums;

namespace ToDoProject.Services;

public static class TaskRepository
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "tasks.json");

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private record TaskItemDto(
        string Name,
        DateTime DueDate,
        EnumTaskPriority Priority,
        EnumConclusionTaskStatus TaskStatus);

    public static List<TaskItem> Load()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        var json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        var dtos = JsonSerializer.Deserialize<List<TaskItemDto>>(json, Options) ?? [];

        return dtos
            .Select(d => TaskItem.FromStorage(d.Name, d.DueDate, d.Priority, d.TaskStatus))
            .ToList();
    }

    public static void Save(IEnumerable<TaskItem> tasks)
    {
        var dtos = tasks
            .Select(t => new TaskItemDto(t.Name, t.DueDate, t.Priority, t.TaskStatus))
            .ToList();

        var json = JsonSerializer.Serialize(dtos, Options);
        File.WriteAllText(FilePath, json);
    }
}
