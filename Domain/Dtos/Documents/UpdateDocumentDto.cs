using Domain.Dtos.TaskItems;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.Documents;

public record UpdateDocumentDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public List<CreateTaskItemDto> Tasks { get; set; }
}
