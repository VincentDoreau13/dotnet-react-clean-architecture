using ShopApi.Application.Common.Interfaces;

namespace ShopApi.Application.Catalog.Commands.UpdateCatalogItemStock;

public record UpdateCatalogItemStockCommand(int Id, int AvailableStock) : ICommand;
