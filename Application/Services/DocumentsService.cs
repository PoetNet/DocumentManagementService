using Database.DocumentManagement.Repositories;
using Domain;
using Domain.Dtos.Documents;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Document = Domain.Entities.Document;

namespace Application.Services;

public interface IDocumentsService
{
    Task<ActionResult> GetPaginated(CancellationToken cancellationToken, int page, int perPage);
    Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ActionResult> CreateAsync(CreateDocumentDto createDto, CancellationToken cancellationToken);
    Task<ActionResult> UpdateAsync(Guid id, CreateDocumentDto createDto, CancellationToken cancellationToken);
    Task<ActionResult> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ActionResult> DeleteAllAsync(CancellationToken cancellationToken);
}

public class DocumentsService(
    DocumentsRepository documentRepository,
    TaskItemsRepository taskItemsRepository)
    : IDocumentsService
{
    public async Task<ActionResult> CreateAsync(CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        List<TaskItem> tasks = createDto.Tasks
            .Select(x => new TaskItem(x.Name) { Status = x.Status })
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

    public async Task<ActionResult> UpdateAsync(Guid id, CreateDocumentDto createDto, CancellationToken cancellationToken)
    {
        var documentToUpdate = await documentRepository.GetByIdAsync(id, cancellationToken);

        if (documentToUpdate == null)
        {
            return new NotFoundObjectResult($"Document with id '{id}' is not found");
        }

        List<TaskItem> tasks = createDto.Tasks
            .Select(x => new TaskItem(x.Name) { Status = x.Status })
            .ToList();

        var activeTasks = tasks
            .Where(x => x.Status == Status.InProgress)
            .ToList();

        if (activeTasks.Count > 1 || activeTasks.Count < 1)
        {
            return new BadRequestObjectResult("We must have only an active task for a document in the same time");
        }

        await taskItemsRepository.DeleteByDocumentIdAsync(documentToUpdate.Id, cancellationToken);
        await documentRepository.UpdateAsync(documentToUpdate, cancellationToken);

        foreach (var task in tasks)
        {
            task.Document = documentToUpdate;
        }

        await taskItemsRepository.CreateRangeAsync(tasks, cancellationToken);

        for (int i = 1; i < tasks.Count; i++)
        {
            tasks[i].PreviousTaskId = tasks[i - 1].Id;
        }

        await taskItemsRepository.UpdateRangeAsync(tasks, cancellationToken);

        TaskItem taskItem = documentToUpdate.Tasks.First(x => x.Status == Status.InProgress);
        var result = new GetDocumentDto()
        {
            Id = documentToUpdate.Id,
            Status = documentToUpdate.Status,
            ActiveTask = new GetTaskItemDto()
            {
                Id = taskItem.Id,
                Status = taskItem.Status,
                Name = taskItem.Name,
                PreviousTaskId = taskItem.PreviousTaskId
            },
            Tasks = documentToUpdate.Tasks
                    .Where(x => x.Status != Status.InProgress)
                    .Select(x => new GetSimpleTaskItemDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PreviousTaskId = x.PreviousTaskId
                    })
                    .ToList()
        };

        return new OkObjectResult(result);
    }

    public async Task<ActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetByIdAsync(id, cancellationToken);

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

    public async Task<ActionResult> GetPaginated(CancellationToken cancellationToken,
        int page = Constants.MIN_PAGE,
        int perPage = Constants.MIN_PER_PAGE)
    {
        var documents = await documentRepository.GetPaginatedDocuments(cancellationToken, page, perPage);


        var result = documents.Select(x =>
        {
            TaskItem? taskItem = x.Tasks.First(t => t.Status == Status.InProgress);

            return new GetDocumentDto()
            {
                Id = x.Id,
                Status = x.Status,
                ActiveTask = new GetTaskItemDto()
                {
                    Id = taskItem.Id,
                    Status = taskItem.Status,
                    Name = taskItem.Name,
                    PreviousTaskId = taskItem.PreviousTaskId
                },
                Tasks = x.Tasks
                    .Where(x => x.Status != Status.InProgress)
                    .Select(x => new GetSimpleTaskItemDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PreviousTaskId = x.PreviousTaskId
                    })
                    .ToList()
            };
        }).ToList();

        return new OkObjectResult(result);
    }

    public async Task<ActionResult> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var deletedDocumentId = await documentRepository.DeleteAsync(id, cancellationToken);

        if (deletedDocumentId == null)
        {
            return new NotFoundObjectResult($"Document with id '{id}' is not found");
        }

        return new OkObjectResult(deletedDocumentId);
    }

    public async Task<ActionResult> DeleteAllAsync(CancellationToken cancellationToken)
    {
        await documentRepository.DeleteAllAsync(cancellationToken);
        return new OkResult();
    }
}
