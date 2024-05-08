using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DocumentManagement.Repositories;

public class TaskItemsRepository : BaseRepository<TaskItem, DocumentManagementDbContext>
{
    private readonly DocumentManagementDbContext _context;

    public TaskItemsRepository(DocumentManagementDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetNewActiveByDocumentIdAsync(Guid previousTaskId, Guid documentId, CancellationToken cancellationToken)
    {
        return await _context.Set<TaskItem>()
            .Where(x => x.PreviousTaskId == previousTaskId && x.Document.Id == documentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public new async Task<TaskItem?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<TaskItem>()
            .Include(x => x.Document)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
