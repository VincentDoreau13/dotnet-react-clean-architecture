using System.Linq.Expressions;
using ShopApi.Domain.Common;

namespace ShopApi.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    TEntity Add(TEntity entity);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    void Delete(TEntity entity);

    Task<bool> ExistAsync(int id, CancellationToken cancellationToken = default);

    Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    void Update(TEntity entity);
}
