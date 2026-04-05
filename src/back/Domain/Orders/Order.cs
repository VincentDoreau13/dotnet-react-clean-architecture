using ShopApi.Domain.Common;
using ShopApi.Domain.Orders.Events;

namespace ShopApi.Domain.Orders;

public class Order : BaseEntity, IAuditable
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Order Create(IEnumerable<(int CatalogItemId, int Quantity, decimal UnitPrice)> lines)
    {
        var order = new Order();

        foreach (var (catalogItemId, quantity, unitPrice) in lines)
            order._items.Add(OrderItem.Create(catalogItemId, quantity, unitPrice));

        order.AddDomainEvent(new OrderCreatedDomainEvent(order));

        return order;
    }

    public void MarkForDeletion()
    {
        AddDomainEvent(new OrderDeletedDomainEvent(this));
    }
}
