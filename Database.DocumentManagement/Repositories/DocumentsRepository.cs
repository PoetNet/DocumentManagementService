using Domain.Entities;

namespace Database.DocumentManagement.Repositories;

public class DocumentsRepository : BaseRepository<Document, DocumentManagementDbContext>
{
    public DocumentsRepository(DocumentManagementDbContext context) : base(context)
    { }
}
