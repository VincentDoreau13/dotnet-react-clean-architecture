using MediatR;
using ShopApi.Application.Orders.DTOs;

namespace ShopApi.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto>;
