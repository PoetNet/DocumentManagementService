using System.ComponentModel.DataAnnotations;
using Domain.Dtos.TaskItems;

namespace Domain.Dtos.Documents;

public record CreateDocumentDto
{
    [Required]
    public List<CreateTaskItemDto> Tasks { get; set; }
}
