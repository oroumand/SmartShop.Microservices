using Microsoft.EntityFrameworkCore;
using SmartShop.Catalog.Core.Domain.Products;
using SmartShop.Catalog.Infra.Data.Products;

namespace SmartShop.Catalog.Infra.Data;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}
