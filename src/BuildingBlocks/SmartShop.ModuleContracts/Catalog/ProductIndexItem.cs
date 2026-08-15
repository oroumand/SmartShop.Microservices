namespace SmartShop.ModuleContracts.Catalog;

public sealed record ProductIndexItem(
    Guid ProductId,
    string Name,
    string Description,
    string Category,
    decimal Price,
    bool IsActive);
