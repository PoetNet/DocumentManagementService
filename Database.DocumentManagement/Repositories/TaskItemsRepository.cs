using Domain.Entities;

namespace Database.DocumentManagement.Repositories;

public class TaskItemsRepository : BaseRepository<TaskItem, DocumentManagementDbContext>
{
    public TaskItemsRepository(DocumentManagementDbContext context) : base(context)
    { }
}
