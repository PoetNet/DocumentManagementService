using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.TaskItems;

public record CreateTaskItemDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public Status Status { get; set; } = Status.Backlog;
}
