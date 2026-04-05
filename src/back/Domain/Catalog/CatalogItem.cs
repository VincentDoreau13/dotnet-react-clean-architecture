using ShopApi.Domain.Common;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Domain.Catalog;

public class CatalogItem : BaseEntity, IAuditable
{
    private CatalogItem() { }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int AvailableStock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static CatalogItem Create(string name, string description, decimal price, int availableStock) =>
        new()
        {
            Name = name,
            Description = description,
            Price = price,
            AvailableStock = availableStock,
        };

    public void RemoveStock(int quantity)
    {
        if (AvailableStock - quantity < 0)
            throw new FunctionalException("INSUFFICIENT_STOCK",
                $"Not enough stock for '{Name}'. Available: {AvailableStock}, requested: {quantity}.");

        AvailableStock -= quantity;
    }
}
