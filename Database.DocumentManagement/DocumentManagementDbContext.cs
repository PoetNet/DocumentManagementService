﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DocumentManagement;

public class DocumentManagementDbContext : DbContext
{
    public DocumentManagementDbContext(DbContextOptions<DocumentManagementDbContext> options) : base(options)
    { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(@"Data Source=..\DocumentManagementDb.db",
            b => b.MigrationsAssembly("Database.DocumentManagement.Migrations"));
    }
}
