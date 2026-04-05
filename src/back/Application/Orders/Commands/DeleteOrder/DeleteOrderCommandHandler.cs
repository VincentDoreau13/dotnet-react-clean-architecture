using MediatR;
using ShopApi.Application.Orders.Interfaces;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetOrderWithItemsByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("NOT_FOUND", "Order", command.Id);

        order.MarkForDeletion();

        orderRepository.Delete(order);
        await orderRepository.SaveEntitiesAsync(cancellationToken);
    }
}
