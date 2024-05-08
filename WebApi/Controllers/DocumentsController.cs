using Application.Services;
using Domain.Dtos.Documents;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController(IDocumentsService documentsService)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<GetDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPaginated([FromQuery] int page, [FromQuery] int perPage, CancellationToken cancellationToken)
    {
        return await documentsService.GetPaginated(cancellationToken, page, perPage);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetDocumentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await documentsService.GetByIdAsync(id, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<ActionResult> PostAsync(CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        return await documentsService.CreateAsync(createDto, cancellationToken);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetDocumentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateAsync(Guid id, CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        return await documentsService.UpdateAsync(id, createDto, cancellationToken);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteAllAsync(CancellationToken cancellationToken)
    {
        return await documentsService.DeleteAllAsync(cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await documentsService.DeleteByIdAsync(id, cancellationToken);
    }

}