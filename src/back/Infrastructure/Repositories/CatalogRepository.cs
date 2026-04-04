using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Catalog.Interfaces;
using ShopApi.Domain.Catalog;
using ShopApi.Infrastructure.Data;
using ShopApi.Infrastructure.Repositories.Common;

namespace ShopApi.Infrastructure.Repositories;

public class CatalogRepository(AppDbContext context) : EfRepository<CatalogItem>(context), ICatalogRepository
{
    public async Task<IReadOnlyList<CatalogItem>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .ToListAsync(cancellationToken);
    }

}
