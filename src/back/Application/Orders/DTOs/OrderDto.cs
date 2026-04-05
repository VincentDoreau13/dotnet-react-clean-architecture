using ShopApi.Domain.Catalog;
using ShopApi.Domain.Orders;

namespace ShopApi.Application.Orders.DTOs;

public record OrderDto(
    int Id,
    IReadOnlyList<OrderItemDto> Items,
    decimal TotalPrice,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static OrderDto FromEntity(Order order, IReadOnlyDictionary<int, CatalogItem> catalogItemsById)
    {
        var items = order.Items
            .Select(orderItem => OrderItemDto.FromEntity(
                orderItem,
                catalogItemsById.TryGetValue(orderItem.CatalogItemId, out var catalogItem)
                    ? catalogItem.Name
                    : "Unknown"))
            .ToList();

        var totalPrice = order.Items.Sum(orderItem => orderItem.UnitPrice * orderItem.Quantity);

        return new OrderDto(order.Id, items, totalPrice, order.CreatedAt, order.UpdatedAt);
    }
}
