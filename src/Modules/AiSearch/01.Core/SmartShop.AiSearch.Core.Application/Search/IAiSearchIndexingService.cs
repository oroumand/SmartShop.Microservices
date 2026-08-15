namespace SmartShop.AiSearch.Core.Application.Search;

public interface IAiSearchIndexingService
{
    Task<IndexProductsResult> ReindexProductsAsync(
        CancellationToken cancellationToken = default);
}
