using ShopApi.Domain.Common;

namespace ShopApi.Domain.Orders;

public class OrderItem : BaseEntity
{
    private OrderItem() { }

    public int CatalogItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    internal static OrderItem Create(int catalogItemId, int quantity, decimal unitPrice) =>
        new()
        {
            CatalogItemId = catalogItemId,
            Quantity = quantity,
            UnitPrice = unitPrice,
        };
}
