namespace SmartShop.AiSearch.Core.Application.Search;

public interface IAiSearchQueryService
{
    Task<IReadOnlyList<ProductSearchResultDto>> SearchAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default);
}
