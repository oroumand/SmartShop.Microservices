namespace SmartShop.AiSearch.Core.Domain.Search;

public sealed record ProductSearchResult(
    Guid ProductId,
    string Name,
    string Description,
    string Category,
    decimal Price,
    double Score);
