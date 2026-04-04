using MediatR;
using ShopApi.Application.Catalog.DTOs;

namespace ShopApi.Application.Catalog.Queries.GetCatalogItems;

public record GetCatalogItemsQuery : IRequest<IEnumerable<CatalogItemDto>>;
