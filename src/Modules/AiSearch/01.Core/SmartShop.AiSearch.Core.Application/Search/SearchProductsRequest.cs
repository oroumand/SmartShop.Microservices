namespace SmartShop.AiSearch.Core.Application.Search;

public sealed record SearchProductsRequest(
    string Query,
    int Limit);
