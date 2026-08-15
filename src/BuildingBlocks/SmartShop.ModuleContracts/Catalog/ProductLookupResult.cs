namespace SmartShop.ModuleContracts.Catalog;

public sealed record ProductLookupResult(
    Guid ProductId,
    string Name,
    decimal Price,
    bool IsActive);
