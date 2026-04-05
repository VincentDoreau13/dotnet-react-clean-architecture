using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopApi.Application.Common.Interfaces;
using ShopApi.Domain.Common;

namespace ShopApi.Infrastructure.Strategies;

public class MustHaveAuditableBeforeSavingStrategy(IIdentityService identityService) : IBeforeSavingDbContextStrategy
{
    private static readonly EntityState[] ModifiedStates =
    [
        EntityState.Added,
        EntityState.Modified
    ];

    public bool CanExecute(EntityEntry entityEntry)
        => entityEntry?.Entity is IAuditable && ModifiedStates.Contains(entityEntry.State);

    public void Execute(EntityEntry entityEntry)
    {
        var now = DateTime.UtcNow;
        var userIdentity = identityService.GetUserIdentity();

        if (entityEntry.State == EntityState.Added)
        {
            entityEntry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
            entityEntry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = userIdentity;
        }

        entityEntry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
        entityEntry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = userIdentity;
    }
}
