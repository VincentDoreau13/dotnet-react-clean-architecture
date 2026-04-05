using MediatR;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Application.Orders.DTOs;
using ShopApi.Application.Orders.Interfaces;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler(
    IOrderRepository orderRepository,
    ICatalogRepository catalogRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetOrderWithItemsByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("NOT_FOUND", nameof(Domain.Orders.Order), request.Id);

        var catalogItemIds = order.Items.Select(orderItem => orderItem.CatalogItemId).ToList();
        var catalogItems = await catalogRepository.GetItemsByIdsAsync(catalogItemIds, cancellationToken);
        var catalogItemsById = catalogItems.ToDictionary(catalogItem => catalogItem.Id);

        return OrderDto.FromEntity(order, catalogItemsById);
    }
}
