using Domain.Enums;
using Newtonsoft.Json;

namespace Domain.Entities;

public record GetSimpleTaskItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? PreviousTaskId  { get; set; }
}
