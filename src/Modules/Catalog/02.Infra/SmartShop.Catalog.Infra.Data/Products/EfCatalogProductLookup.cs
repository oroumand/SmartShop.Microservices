using Microsoft.EntityFrameworkCore;
using SmartShop.ModuleContracts.Catalog;

namespace SmartShop.Catalog.Infra.Data.Products;

public sealed class EfCatalogProductLookup(CatalogDbContext dbContext) : ICatalogProductLookup
{
    public async Task<ProductLookupResult?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == productId && product.IsActive)
            .Select(product => new ProductLookupResult(
                product.Id,
                product.Name,
                product.Price,
                product.IsActive))
            .SingleOrDefaultAsync(cancellationToken);
}
