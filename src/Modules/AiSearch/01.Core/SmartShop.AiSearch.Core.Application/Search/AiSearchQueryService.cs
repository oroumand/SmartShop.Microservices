namespace SmartShop.AiSearch.Core.Application.Search;

public sealed class AiSearchQueryService(
    ITextEmbeddingGenerator embeddingGenerator,
    IProductVectorStore vectorStore) : IAiSearchQueryService
{
    private const int DefaultLimit = 5;

    public async Task<IReadOnlyList<ProductSearchResultDto>> SearchAsync(
        SearchProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = request.Query.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search query is required.", nameof(request));
        }

        var limit = request.Limit <= 0
            ? DefaultLimit
            : request.Limit;

        var queryEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(
            query,
            cancellationToken);

        var results = await vectorStore.SearchProductsAsync(
            queryEmbedding,
            limit,
            cancellationToken);

        return results
            .Select(result => new ProductSearchResultDto(
                result.ProductId,
                result.Name,
                result.Description,
                result.Category,
                result.Price,
                result.Score))
            .ToList();
    }
}
