using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DocumentManagement.Repositories;

public class DocumentsRepository : BaseRepository<Document, DocumentManagementDbContext>
{
    private readonly DocumentManagementDbContext _context;

    public DocumentsRepository(DocumentManagementDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Document?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Document>()
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
