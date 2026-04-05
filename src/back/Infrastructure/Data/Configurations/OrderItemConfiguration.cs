using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApi.Domain.Orders;

namespace ShopApi.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> orderItemBuilder)
    {
        orderItemBuilder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        orderItemBuilder.Property(orderItem => orderItem.UnitPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        orderItemBuilder.HasOne<Domain.Catalog.CatalogItem>()
            .WithMany()
            .HasForeignKey(orderItem => orderItem.CatalogItemId)
            .OnDelete(DeleteBehavior.Restrict);

        orderItemBuilder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_OrderItem_Quantity", "\"Quantity\" > 0");
            table.HasCheckConstraint("CK_OrderItem_UnitPrice", "\"UnitPrice\" > 0");
        });
    }
}
