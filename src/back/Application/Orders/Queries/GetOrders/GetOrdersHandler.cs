using MediatR;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Application.Orders.DTOs;
using ShopApi.Application.Orders.Interfaces;

namespace ShopApi.Application.Orders.Queries.GetOrders;

public class GetOrdersHandler(
    IOrderRepository orderRepository,
    ICatalogRepository catalogRepository) : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllOrdersWithItemsAsync(cancellationToken);
        var catalogItemIds = orders.SelectMany(order => order.Items.Select(orderItem => orderItem.CatalogItemId)).Distinct().ToList();
        var catalogItems = await catalogRepository.GetItemsByIdsAsync(catalogItemIds, cancellationToken);
        var catalogItemsById = catalogItems.ToDictionary(catalogItem => catalogItem.Id);

        return orders.Select(order => OrderDto.FromEntity(order, catalogItemsById));
    }
}
