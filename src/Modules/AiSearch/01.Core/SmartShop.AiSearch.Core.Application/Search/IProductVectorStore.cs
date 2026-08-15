using SmartShop.AiSearch.Core.Domain.Search;

namespace SmartShop.AiSearch.Core.Application.Search;

public interface IProductVectorStore
{
    Task UpsertProductsAsync(
        IReadOnlyList<ProductSearchDocument> documents,
        IReadOnlyList<float[]> embeddings,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductSearchResult>> SearchProductsAsync(
        float[] queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default);
}
