using MediatR;
using ShopApi.Application.Orders.DTOs;

namespace ShopApi.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<IEnumerable<OrderDto>>;
