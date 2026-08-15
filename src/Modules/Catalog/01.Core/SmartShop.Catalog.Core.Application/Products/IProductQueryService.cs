namespace SmartShop.Catalog.Core.Application.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductDto>> SearchProductsAsync(
        string query,
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
