using MediatR;
using ShopApi.Domain.Common;
using ShopApi.Infrastructure.Data;

namespace ShopApi.Infrastructure.Data;

public static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, AppDbContext applicationDbContext)
    {
        var domainEntities = applicationDbContext.ChangeTracker
            .Entries<BaseEntity>()
            .Where(entity => entity.Entity.DomainEvents?.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entity => entity.Entity.DomainEvents ?? [])
            .ToList();

        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (INotification domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}
