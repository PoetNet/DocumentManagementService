using Database.DocumentManagement.Repositories;
using Domain.Dtos.Documents;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services;

public interface IDocumentsService
{
    Task<ActionResult> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ActionResult> CreateAsync(CreateDocumentDto createDto, CancellationToken cancellationToken);
}

public class DocumentsService(
    DocumentsRepository documentRepository,
    TaskItemsRepository taskItemsRepository)
    : IDocumentsService
{
    public async Task<ActionResult> CreateAsync(CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        List<TaskItem> tasks = createDto.Tasks
            .Select(x => new TaskItem(x.Name) {Status = x.Status})
            .ToList();

        var activeTasks = tasks
            .Where(x => x.Status == Status.InProgress)
            .ToList();

        if (activeTasks.Count > 1 || activeTasks.Count < 1)
        {
            return new BadRequestObjectResult("We must have only an active task for a document in the same time");
        }

        Document newDocument = new();

        Guid documentId = await documentRepository.CreateAsync(newDocument, cancellationToken);

        foreach (var task in tasks)
        {
            task.Document = newDocument;
        }

        await taskItemsRepository.CreateRangeAsync(tasks, cancellationToken);

        for (int i = 1; i < tasks.Count; i++)
        {
            tasks[i].PreviousTaskId = tasks[i - 1].Id;
        }

        await taskItemsRepository.UpdateRangeAsync(tasks, cancellationToken);

        return new OkObjectResult(documentId);
    }

    public async Task<ActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetAsync(id, cancellationToken);

        if (document == null)
        {
            return new NotFoundObjectResult($"Document with id '{id}' is not found");
        }

        TaskItem? taskItem = document.Tasks.FirstOrDefault(x => x.Status == Status.InProgress);

        GetDocumentDto documentDto = new()
        {
            Id = document.Id,
            Status = document.Status,
            ActiveTask = taskItem == null ? null :
                new GetTaskItemDto()
                {
                    Id = taskItem.Id,
                    Status = taskItem.Status,
                    Name = taskItem.Name,
                    PreviousTaskId = taskItem.PreviousTaskId
                },
            Tasks = document.Tasks
                .Where(x => x.Status != Status.InProgress)
                .Select(x => new GetSimpleTaskItemDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PreviousTaskId = x.PreviousTaskId
                })
                .ToList()
        };

        return new OkObjectResult(documentDto);
    }

}
