using Domain.Dtos.TaskItems;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.Documents;

public record CreateDocumentDto
{
    [Required]
    public List<CreateTaskItemDto> Tasks { get; set; }
}
