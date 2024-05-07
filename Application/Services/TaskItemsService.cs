using Database.DocumentManagement.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Application.Services;

public interface ITaskItemsService
{
    Task<ActionResult> ExecuteAsync(Guid id, CancellationToken cancellationToken);
    Task<ActionResult> CancelAsync(Guid id, CancellationToken cancellationToken);
}

public class TaskItemsService(
    DocumentsRepository documentRepository,
    TaskItemsRepository taskItemsRepository)
    : ITaskItemsService
{
    public async Task<ActionResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await taskItemsRepository.GetAsync(id, cancellationToken);

        if (taskItem == null)
        {
            return new NotFoundObjectResult($"Task with id '{id}' is not found");
        }
        if (IsTaskActive(taskItem))
        {
            return new BadRequestObjectResult($"Task with id '{id}' is not active. Please, choose an active task");
        }

        var document = taskItem.Document;
        var newActiveTask = await taskItemsRepository.GetNewActiveByDocumentIdAsync(taskItem.Id, document.Id, cancellationToken);

        taskItem.Status = Status.InOperationalArchive;

        if (newActiveTask == null)
        {
            document.Status = Status.InOperationalArchive;
            await documentRepository.UpdateAsync(document, cancellationToken);
            return new OkObjectResult($"All task are completed for the document with id {document.Id}.\nIt brought to the operational archive");
        }

        newActiveTask.Status = Status.InProgress;
        await documentRepository.UpdateAsync(document, cancellationToken);

        return new OkObjectResult($"Task with id '{id}' is executed");
    }    
    
    public async Task<ActionResult> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await taskItemsRepository.GetAsync(id, cancellationToken);

        if (taskItem == null)
        {
            return new NotFoundObjectResult($"Task with id '{id}' is not found");
        }
        if (IsTaskActive(taskItem))
        {
            return new BadRequestObjectResult($"Task with id '{id}' is not active. Please, choose an active task");
        }

        var document = taskItem.Document;
        taskItem.Status = Status.Canceled;
        document.Status = Status.Canceled;

        await documentRepository.UpdateAsync(document, cancellationToken);

        return new OkObjectResult($"Document with id '{document.Id}' is canceled by canceling task with id '{id}'");
    }

    private bool IsTaskActive(TaskItem taskItem) => taskItem.Status != Status.InProgress;
}
