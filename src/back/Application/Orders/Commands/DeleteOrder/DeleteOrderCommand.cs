using ShopApi.Application.Common.Interfaces;

namespace ShopApi.Application.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(int Id) : ICommand;
