using MediatR;
using ShopApi.Application.Catalog.DTOs;
using ShopApi.Application.Catalog.Interfaces;

namespace ShopApi.Application.Catalog.Queries.GetCatalogItems;

public class GetCatalogItemsHandler(ICatalogRepository repository) : IRequestHandler<GetCatalogItemsQuery, IEnumerable<CatalogItemDto>>
{
    public async Task<IEnumerable<CatalogItemDto>> Handle(GetCatalogItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetAllItemsAsync(cancellationToken);
        return items.Select(CatalogItemDto.FromEntity);
    }
}
