using MediatR;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Application.Orders.DTOs;
using ShopApi.Application.Orders.Interfaces;
using ShopApi.Domain.Exceptions;
using ShopApi.Domain.Orders;

namespace ShopApi.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    ICatalogRepository catalogRepository) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var catalogItemIds = command.Items.Select(orderItemCommand => orderItemCommand.CatalogItemId).Distinct().ToList();
        var catalogItems = await catalogRepository.GetItemsByIdsAsync(catalogItemIds, cancellationToken);
        var catalogItemsById = catalogItems.ToDictionary(catalogItem => catalogItem.Id);

        foreach (var orderItemCommand in command.Items)
        {
            if (!catalogItemsById.TryGetValue(orderItemCommand.CatalogItemId, out var catalogItem))
                throw new NotFoundException("NOT_FOUND", "CatalogItem", orderItemCommand.CatalogItemId);

            if (orderItemCommand.Quantity > catalogItem.AvailableStock)
                throw new FunctionalException("INSUFFICIENT_STOCK",
                    $"Not enough stock for '{catalogItem.Name}'. Available: {catalogItem.AvailableStock}, requested: {orderItemCommand.Quantity}.");
        }

        var lines = command.Items
            .Select(orderItemCommand =>
            {
                var catalogItem = catalogItemsById[orderItemCommand.CatalogItemId];
                return (catalogItem.Id, orderItemCommand.Quantity, catalogItem.Price);
            })
            .ToList();

        var order = Order.Create(lines);

        orderRepository.Add(order);
        await orderRepository.SaveEntitiesAsync(cancellationToken);

        return OrderDto.FromEntity(order, catalogItemsById);
    }
}
