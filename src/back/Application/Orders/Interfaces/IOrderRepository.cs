using ShopApi.Application.Common.Interfaces;
using ShopApi.Domain.Orders;

namespace ShopApi.Application.Orders.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetAllOrdersWithItemsAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetOrderWithItemsByIdAsync(int id, CancellationToken cancellationToken = default);
}
