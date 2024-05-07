using Application.Services;
using Domain.Dtos.Documents;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskItemsController(ITaskItemsService taskItemsService)
{
    [HttpPatch("{id:guid}/execute")]
    public async Task<ActionResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await taskItemsService.ExecuteAsync(id, cancellationToken); ;
    }    
    
    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        return await taskItemsService.CancelAsync(id, cancellationToken); ;
    }
}
