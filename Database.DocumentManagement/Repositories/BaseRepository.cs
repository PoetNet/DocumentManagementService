using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.DocumentManagement.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<List<Guid>> CreateRangeAsync(List<T> entities, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<List<T>> UpdateRangeAsync(List<T> entities, CancellationToken cancellationToken);

    Task<T?> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAllAsync(CancellationToken cancellationToken);
}

public abstract class BaseRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class, IEntity
    where TContext : DbContext
{
    private readonly TContext context;
    public BaseRepository(TContext context)
    {
        this.context = context;
    }
    public async Task<Guid> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<List<Guid>> CreateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entities.Select(x => x.Id).ToList();
    }

    public async Task<TEntity?> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null)
        {
            return entity;
        }

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        var entities = context.Set<TEntity>();
        context.Set<TEntity>().RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        await context.SaveChangesAsync(cancellationToken);
        return entities;
    }
}
