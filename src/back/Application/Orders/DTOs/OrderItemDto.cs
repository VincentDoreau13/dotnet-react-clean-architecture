using ShopApi.Domain.Orders;

namespace ShopApi.Application.Orders.DTOs;

public record OrderItemDto(
    int Id,
    int CatalogItemId,
    string CatalogItemName,
    int Quantity,
    decimal UnitPrice)
{
    public static OrderItemDto FromEntity(OrderItem orderItem, string catalogItemName) =>
        new(orderItem.Id, orderItem.CatalogItemId, catalogItemName, orderItem.Quantity, orderItem.UnitPrice);
}
