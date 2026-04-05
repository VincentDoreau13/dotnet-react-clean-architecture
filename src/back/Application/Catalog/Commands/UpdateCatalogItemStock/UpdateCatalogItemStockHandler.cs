using MediatR;
using ShopApi.Application.Catalog.Interfaces;

namespace ShopApi.Application.Catalog.Commands.UpdateCatalogItemStock;

public class UpdateCatalogItemStockHandler(ICatalogRepository repository)
    : IRequestHandler<UpdateCatalogItemStockCommand>
{
    public async Task Handle(UpdateCatalogItemStockCommand command, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(command.Id, cancellationToken);

        item.SetStock(command.AvailableStock);

        repository.Update(item);
        await repository.SaveEntitiesAsync(cancellationToken);
    }
}
