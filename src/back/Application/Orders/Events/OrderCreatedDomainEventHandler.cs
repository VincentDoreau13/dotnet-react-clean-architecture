using MediatR;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Domain.Orders.Events;

namespace ShopApi.Application.Orders.Events;

public class OrderCreatedDomainEventHandler(ICatalogRepository catalogRepository)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var catalogItemIds = notification.Order.Items.Select(orderItem => orderItem.CatalogItemId).ToList();
        var catalogItems = await catalogRepository.GetItemsByIdsAsync(catalogItemIds, cancellationToken);
        var catalogItemsById = catalogItems.ToDictionary(catalogItem => catalogItem.Id);

        foreach (var orderItem in notification.Order.Items)
        {
            var catalogItem = catalogItemsById[orderItem.CatalogItemId];
            catalogItem.RemoveStock(orderItem.Quantity);
            catalogRepository.Update(catalogItem);
        }

        await catalogRepository.SaveChangesAsync(cancellationToken);
    }
}
