using Domain.Enums;

namespace Domain.Entities;

public class Document : IEntity
{
    public Guid Id { get; set; }

    public Status Status { get; set; } = Status.Backlog;
    public List<TaskItem> Tasks { get; set; } = new();

}
