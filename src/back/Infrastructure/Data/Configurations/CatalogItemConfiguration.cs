using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Catalog;

namespace ShopApi.Infrastructure.Data.Configurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.Property(catalogItem => catalogItem.Name).IsRequired().HasMaxLength(200);
        builder.Property(catalogItem => catalogItem.Description).IsRequired().HasMaxLength(1000);
        builder.Property(catalogItem => catalogItem.Price).HasColumnType("numeric(18,2)");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_CatalogItem_Price", "\"Price\" > 0");
            table.HasCheckConstraint("CK_CatalogItem_AvailableStock", "\"AvailableStock\" >= 0");
        });

        var seed = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new { Id = 1, Name = ".NET Bot Black Sweatshirt",    Description = "Classic black sweatshirt",           Price = 19.5m,  AvailableStock = 100, CreatedAt = seed, UpdatedAt = seed },
            new { Id = 2, Name = ".NET Black & White Mug",       Description = "Coffee mug with .NET logo",          Price = 8.50m,  AvailableStock = 200, CreatedAt = seed, UpdatedAt = seed },
            new { Id = 3, Name = "Prism White T-Shirt",          Description = "Soft cotton t-shirt",                Price = 12.0m,  AvailableStock = 150, CreatedAt = seed, UpdatedAt = seed },
            new { Id = 4, Name = ".NET Foundation T-Shirt",      Description = "Official .NET Foundation t-shirt",   Price = 12.0m,  AvailableStock = 180, CreatedAt = seed, UpdatedAt = seed },
            new { Id = 5, Name = "Roslyn Red Pin",               Description = "Collector pin",                      Price = 8.5m,   AvailableStock = 300, CreatedAt = seed, UpdatedAt = seed }
        );
    }
}
