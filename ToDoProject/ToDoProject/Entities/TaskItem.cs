using System.Text;
using ToDoProject.Domain;
using ToDoProject.Entities.Enums;

namespace ToDoProject.Entities;

public class TaskItem
{
    private string Name { get; set; }
    private DateTime DueDate { get; set; }
    public EnumTaskPriority Priority { get; set; }
    public EnumConclusionTaskStatus TaskStatus { get; set; }

    public TaskItem(string name, DateTime dueDate, EnumTaskPriority priority, EnumConclusionTaskStatus taskStatus = EnumConclusionTaskStatus.WaitingStart)
    {
        ValidateDueDate(dueDate);
        
        Priority = priority;
        Name = name;
        DueDate = dueDate;
        TaskStatus = taskStatus;
    }

    private void ValidateDueDate(DateTime dueDate)
    {
        if (dueDate < DateTime.Today)
        {
            throw new DomainException("Due date cannot be in the past");
        }
    }
    
    public void ChangePriority(EnumTaskPriority priority)
    {
        if (priority == Priority)
        {
            throw new DomainException("The chosen priority is already in use in this task");
        }
        
        Priority = priority;
    }

    public void ChangeDueDate(DateTime dueDate)
    {
        ValidateDueDate(dueDate);
        DueDate = dueDate;
    }

    public void ChangeTaskStatus(EnumConclusionTaskStatus taskStatus)
    {
        if (TaskStatus == EnumConclusionTaskStatus.Completed)
        {
            throw new DomainException("The task status is completed");
        }
        
        if (taskStatus == TaskStatus)
        {
            throw new DomainException("The task status is already in use in this task");
        }
        
        TaskStatus = taskStatus;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Concat("Task name: ", Name));
        sb.AppendLine(string.Concat("Priority: ", Priority.ToString()));
        sb.AppendLine(string.Concat("Due date: ", DueDate.ToString("yyyy-MM-dd HH:mm")));
        
        return sb.ToString();
    }
}