using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Infra.Data.Orders;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", "ordering");

        builder.HasKey(item => item.Id);

        builder.Property<Guid>("OrderId")
            .IsRequired();

        builder.Property(item => item.ProductId)
            .IsRequired();

        builder.Property(item => item.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Ignore(item => item.LineTotal);
    }
}
