using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Catalog.Commands.CreateCatalogItem;
using ShopApi.Application.Catalog.DTOs;
using ShopApi.Application.Catalog.Queries.GetCatalogItemById;
using ShopApi.Application.Catalog.Queries.GetCatalogItems;

namespace ShopApi.API.Controllers;

[ApiController]
[Route("api/catalog")]
[Produces("application/json")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    /// <summary>Get all catalog items</summary>
    [HttpGet("items")]
    [ProducesResponseType(typeof(IEnumerable<CatalogItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCatalogItemsQuery(), cancellationToken));
    }

    /// <summary>Get a catalog item by ID</summary>
    [HttpGet("items/{id:int}", Name = "GetById")]
    [ProducesResponseType(typeof(CatalogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CatalogItemDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCatalogItemByIdQuery(id), cancellationToken));
    }

    /// <summary>Create a catalog item</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CatalogItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CatalogItemDto>> Create(CreateCatalogItemCommand command, CancellationToken cancellationToken)
    {
        var item = await mediator.Send(command, cancellationToken);
        return CreatedAtRoute("GetById", new { id = item.Id }, item);
    }
}
