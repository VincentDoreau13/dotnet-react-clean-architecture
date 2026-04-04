using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ShopApi.Infrastructure.Strategies;

public interface IBeforeSavingDbContextStrategy : IDbContextStrategy
{
    bool CanExecute(EntityEntry entityEntry);

    void Execute(EntityEntry entityEntry);
}
