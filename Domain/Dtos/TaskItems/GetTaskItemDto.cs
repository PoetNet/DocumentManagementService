using Domain.Enums;

namespace Domain.Entities;

public record GetTaskItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Status Status { get; set; }
    public Guid? PreviousTaskId { get; set; }
}
