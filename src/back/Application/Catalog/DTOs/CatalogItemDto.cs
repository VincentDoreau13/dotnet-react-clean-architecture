using ShopApi.Domain.Catalog;

namespace ShopApi.Application.Catalog.DTOs;

public record CatalogItemDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int AvailableStock,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CatalogItemDto FromEntity(CatalogItem item) =>
        new(item.Id, item.Name, item.Description, item.Price,
            item.AvailableStock, item.CreatedAt, item.UpdatedAt);
}
