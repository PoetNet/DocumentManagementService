using System.ComponentModel.DataAnnotations;
using Domain.Dtos.TaskItems;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Dtos.Documents;

public record GetDocumentDto
{
    public Guid Id { get; set; }
    public Status Status { get; set; }
    public GetTaskItemDto? ActiveTask { get; set; }
    public List<GetSimpleTaskItemDto> Tasks { get; set; }
}
