using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopApi.Application.Common.Interfaces;
using ShopApi.Domain.Common;
using ShopApi.Domain.Exceptions;
using ShopApi.Infrastructure.Data;

namespace ShopApi.Infrastructure.Repositories.Common;

public abstract class EfRepository<TEntity>(AppDbContext dbContext) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected DbSet<TEntity> DbSet => dbContext.Set<TEntity>();

    public TEntity Add(TEntity entity)
    {
        if (entity.IsTransient())
        {
            return DbSet
                .Add(entity)
                .Entity;
        }

        return entity;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(expression, cancellationToken);
    }

    public void Delete(TEntity entity)
    {
        dbContext.Entry(entity).State = EntityState.Deleted;
    }

    public Task<bool> ExistAsync(int id, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(entity => entity.Id == id, cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        return entity ?? throw new NotFoundException("NOT_FOUND", typeof(TEntity).Name, id);
    }

    public async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveEntitiesAsync(cancellationToken);
    }

    public void Update(TEntity entity)
    {
        dbContext.Entry(entity).State = EntityState.Modified;

        IEnumerable<ReferenceEntry> ownedProperties = dbContext.Entry(entity).References
            .Where(reference => reference.TargetEntry != null && reference.TargetEntry.Metadata.IsOwned());

        foreach (var entityEntry in ownedProperties.Select(reference => reference.TargetEntry).Where(entry => entry != null))
            entityEntry!.State = EntityState.Modified;
    }
}
