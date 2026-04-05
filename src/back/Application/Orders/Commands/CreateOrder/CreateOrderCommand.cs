using ShopApi.Application.Common.Interfaces;
using ShopApi.Application.Orders.DTOs;

namespace ShopApi.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(IReadOnlyList<CreateOrderItemCommand> Items) : ICommand<OrderDto>;

public record CreateOrderItemCommand(int CatalogItemId, int Quantity);
