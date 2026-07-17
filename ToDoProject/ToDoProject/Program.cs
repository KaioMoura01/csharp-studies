using ToDoProject.Domain;
using ToDoProject.Entities;
using ToDoProject.Entities.Enums;
using ToDoProject.Misc;

var tasks = new List<TaskItem>();

Console.WriteLine("=== Welcome to ToDoProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Add task");
    Console.WriteLine("2 - Complete task");
    Console.WriteLine("3 - Remove task");
    Console.WriteLine("4 - List tasks (grouped by priority)");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var name = Utils.ReadString("Task name: ");
                var dueDate = Utils.ReadDate("Due date (dd/MM/yyyy): ");
                var priority = ReadPriority();

                tasks.Add(new TaskItem(name, dueDate, priority));
                Console.WriteLine("Task added successfully!");
                break;
            }
            case 2:
            {
                var task = SelectTask("Select the task to complete");
                if (task is null) break;

                task.ChangeTaskStatus(EnumConclusionTaskStatus.Completed);
                Console.WriteLine("Task completed!");
                break;
            }
            case 3:
            {
                var task = SelectTask("Select the task to remove");
                if (task is null) break;

                tasks.Remove(task);
                Console.WriteLine("Task removed!");
                break;
            }
            case 4:
                ListGroupedByPriority();
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
    catch (DomainException e)
    {
        Console.WriteLine($"Validation error: {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}

return;

EnumTaskPriority ReadPriority()
{
    Console.WriteLine("Priority: 1 - High, 2 - Medium, 3 - Low");
    while (true)
    {
        var choice = Utils.ReadInt("Choose the priority: ");
        switch (choice)
        {
            case 1: return EnumTaskPriority.High;
            case 2: return EnumTaskPriority.Medium;
            case 3: return EnumTaskPriority.Low;
            default: Console.WriteLine("Invalid priority, try again."); break;
        }
    }
}

TaskItem? SelectTask(string label)
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks registered yet.");
        return null;
    }

    for (var i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"[{i + 1}] {tasks[i].Priority} priority, status {tasks[i].TaskStatus}");
        Console.Write(tasks[i]);
    }

    var index = Utils.ReadInt($"{label} (1-{tasks.Count}): ");
    if (index < 1 || index > tasks.Count)
    {
        Console.WriteLine("Invalid task number.");
        return null;
    }

    return tasks[index - 1];
}

void ListGroupedByPriority()
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks registered yet.");
        return;
    }

    var groups = tasks
        .GroupBy(t => t.Priority)
        .OrderBy(g => g.Key);

    foreach (var group in groups)
    {
        Console.WriteLine($"=== {group.Key} priority ===");
        foreach (var task in group)
        {
            Console.WriteLine(task);
            Console.WriteLine($"Status: {task.TaskStatus}");
            Console.WriteLine();
        }
    }
}
