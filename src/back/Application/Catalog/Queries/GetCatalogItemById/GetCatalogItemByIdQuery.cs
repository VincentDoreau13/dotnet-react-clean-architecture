using MediatR;
using ShopApi.Application.Catalog.DTOs;

namespace ShopApi.Application.Catalog.Queries.GetCatalogItemById;

public record GetCatalogItemByIdQuery(int Id) : IRequest<CatalogItemDto>;
