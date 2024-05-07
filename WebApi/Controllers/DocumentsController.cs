using Application.Services;
using Domain.Dtos.Documents;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController(IDocumentService documentService)
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await documentService.GetAsync(id, cancellationToken); ;
    }    
    
    [HttpPost]
    public async Task<ActionResult> PostAsync(CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        return await documentService.CreateAsync(createDto, cancellationToken); ;
    }
}