using Database.DocumentManagement.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DocumentManagement;

public class DocumentManagementDbContext : DbContext
{
    public DocumentManagementDbContext(DbContextOptions<DocumentManagementDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
