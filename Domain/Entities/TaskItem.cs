using Domain.Enums;

namespace Domain.Entities;

public class TaskItem : IEntity
{
    public TaskItem(string name)
    {
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; private set; }
    public Status Status { get; set; } = Status.Backlog;
    public Guid? PreviousTaskId { get; set; }
    public Document Document { get; set; }
}
