using MediatR;
using ShopApi.Application.Catalog.DTOs;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Domain.Catalog;

namespace ShopApi.Application.Catalog.Commands.CreateCatalogItem;

public class CreateCatalogItemHandler(ICatalogRepository repository) : IRequestHandler<CreateCatalogItemCommand, CatalogItemDto>
{
    public async Task<CatalogItemDto> Handle(CreateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        var item = CatalogItem.Create(request.Name, request.Description, request.Price, request.AvailableStock);

        repository.Add(item);
        await repository.SaveEntitiesAsync(cancellationToken);

        return CatalogItemDto.FromEntity(item);
    }
}
