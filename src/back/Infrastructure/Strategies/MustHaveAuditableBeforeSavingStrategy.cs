using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopApi.Domain.Common;

namespace ShopApi.Infrastructure.Strategies;

public class MustHaveAuditableBeforeSavingStrategy : IBeforeSavingDbContextStrategy
{
    private static readonly EntityState[] ModifiedStates =
    [
        EntityState.Added,
        EntityState.Modified,
        EntityState.Deleted
    ];

    public bool CanExecute(EntityEntry entityEntry)
    {
        return entityEntry?.Entity is IAuditable && ModifiedStates.Contains(entityEntry.State);
    }

    public void Execute(EntityEntry entityEntry)
    {
        var now = DateTime.UtcNow;

        if (entityEntry.State == EntityState.Added)
            entityEntry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;

        entityEntry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
    }
}
