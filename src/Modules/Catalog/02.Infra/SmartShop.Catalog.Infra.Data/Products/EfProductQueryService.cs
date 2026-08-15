using Microsoft.EntityFrameworkCore;
using SmartShop.Catalog.Core.Application.Products;

namespace SmartShop.Catalog.Infra.Data.Products;

public sealed class EfProductQueryService(CatalogDbContext dbContext) : IProductQueryService
{
    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductDto>> SearchProductsAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var searchTerm = query.Trim();

        return await dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive &&
                (product.Name.Contains(searchTerm) ||
                 product.Description.Contains(searchTerm) ||
                 product.Category.Contains(searchTerm)))
            .OrderBy(product => product.Name)
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id && product.IsActive)
            .Select(product => new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.IsActive))
            .SingleOrDefaultAsync(cancellationToken);
}
