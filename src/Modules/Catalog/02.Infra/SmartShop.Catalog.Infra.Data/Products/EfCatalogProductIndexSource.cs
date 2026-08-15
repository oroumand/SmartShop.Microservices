using Microsoft.EntityFrameworkCore;
using SmartShop.ModuleContracts.Catalog;

namespace SmartShop.Catalog.Infra.Data.Products;

public sealed class EfCatalogProductIndexSource(CatalogDbContext dbContext) : ICatalogProductIndexSource
{
    public async Task<IReadOnlyList<ProductIndexItem>> GetActiveProductsForIndexAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .Select(product => new ProductIndexItem(
                product.Id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.IsActive))
            .ToListAsync(cancellationToken);
}
