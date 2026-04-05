using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Orders.Commands.CreateOrder;
using ShopApi.Application.Orders.Commands.DeleteOrder;
using ShopApi.Application.Orders.DTOs;
using ShopApi.Application.Orders.Queries.GetOrderById;
using ShopApi.Application.Orders.Queries.GetOrders;

namespace ShopApi.API.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>Get all orders</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetOrdersQuery(), cancellationToken));
    }

    /// <summary>Get an order by ID</summary>
    [HttpGet("{id:int}", Name = "GetOrderById")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetOrderByIdQuery(id), cancellationToken));
    }

    /// <summary>Create an order</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await mediator.Send(command, cancellationToken);
        return CreatedAtRoute("GetOrderById", new { id = order.Id }, order);
    }

    /// <summary>Delete an order</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrderCommand(id), cancellationToken);
        return NoContent();
    }
}
