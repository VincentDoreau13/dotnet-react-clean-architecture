using ShopApi.Application.Common.Interfaces;
using ShopApi.Domain.Catalog;

namespace ShopApi.Application.Catalog.Interfaces;

public interface ICatalogRepository : IRepository<CatalogItem>
{
    Task<IReadOnlyList<CatalogItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
}
