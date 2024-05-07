using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class Document : IEntity
{
    public Guid Id { get; set; }

    public Status Status { get; set; } = Status.InProgress;

    public List<TaskItem> Tasks { get; set; } = new();

}
