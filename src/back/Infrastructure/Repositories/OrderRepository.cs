using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Orders.Interfaces;
using ShopApi.Domain.Orders;
using ShopApi.Infrastructure.Data;
using ShopApi.Infrastructure.Repositories.Common;

namespace ShopApi.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : EfRepository<Order>(context), IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllOrdersWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderWithItemsByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);
    }
}
