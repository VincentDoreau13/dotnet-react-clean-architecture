using MediatR;
using ShopApi.Application.Catalog.DTOs;
using ShopApi.Application.Catalog.Interfaces;

namespace ShopApi.Application.Catalog.Queries.GetCatalogItemById;

public class GetCatalogItemByIdHandler(ICatalogRepository repository) : IRequestHandler<GetCatalogItemByIdQuery, CatalogItemDto>
{
    public async Task<CatalogItemDto> Handle(GetCatalogItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return CatalogItemDto.FromEntity(item);
    }
}
