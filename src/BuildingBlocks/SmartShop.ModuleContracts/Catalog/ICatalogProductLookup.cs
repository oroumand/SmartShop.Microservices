namespace SmartShop.ModuleContracts.Catalog;

public interface ICatalogProductLookup
{
    Task<ProductLookupResult?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
