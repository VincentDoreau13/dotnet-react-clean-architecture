using ShopApi.Application.Catalog.DTOs;
using ShopApi.Application.Common.Interfaces;

namespace ShopApi.Application.Catalog.Commands.CreateCatalogItem;

public record CreateCatalogItemCommand(
    string Name,
    string Description,
    decimal Price,
    int AvailableStock) : ICommand<CatalogItemDto>;
