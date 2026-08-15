namespace SmartShop.AiSearch.Core.Application.Search;

public sealed record ProductSearchResultDto(
    Guid ProductId,
    string Name,
    string Description,
    string Category,
    decimal Price,
    double Score);
