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
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

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
        if (quantity <= 0)
            throw new FunctionalException("INVALID_QUANTITY",
                "Stock quantity to remove must be greater than 0.");

        if (AvailableStock - quantity < 0)
            throw new FunctionalException("INSUFFICIENT_STOCK", "Not enough stock for this item.");

        AvailableStock -= quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new FunctionalException("INVALID_QUANTITY",
                "Stock quantity to add must be greater than 0.");

        if (AvailableStock > int.MaxValue - quantity)
            throw new FunctionalException("STOCK_OVERFLOW",
                "Adding this quantity would exceed the maximum stock limit.");

        AvailableStock += quantity;
    }

    public void SetStock(int quantity)
    {
        if (quantity < 0)
            throw new FunctionalException("NEGATIVE_QUANTITY", "Stock quantity cannot be negative.");
        if (quantity > 1_000_000)
            throw new FunctionalException("STOCK_LIMIT_EXCEEDED", "Stock cannot exceed 1 000 000.");
        AvailableStock = quantity;
    }
}
