namespace SmartShop.ModuleContracts.Catalog;

public interface ICatalogProductIndexSource
{
    Task<IReadOnlyList<ProductIndexItem>> GetActiveProductsForIndexAsync(
        CancellationToken cancellationToken = default);
}
