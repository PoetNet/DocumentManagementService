using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Database.DocumentManagement.Repositories;

public class TaskItemsRepository : BaseRepository<TaskItem, DocumentManagementDbContext>
{
    private readonly DocumentManagementDbContext _context;

    public TaskItemsRepository(DocumentManagementDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetActiveByDocumentIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<TaskItem>()
            .Where(x => x.Status == Status.InProgress && x.Document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
