namespace SmartShop.Catalog.Core.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    bool IsActive);
