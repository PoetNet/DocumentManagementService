using Domain;
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

    public async Task<Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Document>()
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Document>> GetPaginatedDocuments(
        CancellationToken cancellationToken, int page, int perPage)
    {
        if (page < Constants.MIN_PAGE) page = Constants.MIN_PAGE;
        if (perPage < 1) perPage = Constants.MIN_PER_PAGE;

        return await _context.Set<Document>()
            .Include(p => p.Tasks)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public new async Task<Guid?> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<Document>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null)
        {
            return null;
        }

        _context.Set<Document>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
