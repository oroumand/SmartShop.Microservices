using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartShop.Catalog.Core.Domain.Products;

namespace SmartShop.Catalog.Infra.Data.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "catalog");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(product => product.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2);

        builder.Property(product => product.IsActive)
            .IsRequired();
    }
}
