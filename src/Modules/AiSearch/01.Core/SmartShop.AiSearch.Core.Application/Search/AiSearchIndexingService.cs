using SmartShop.AiSearch.Core.Domain.Search;
using SmartShop.ModuleContracts.Catalog;

namespace SmartShop.AiSearch.Core.Application.Search;

public sealed class AiSearchIndexingService(
    ICatalogProductIndexSource productIndexSource,
    ITextEmbeddingGenerator embeddingGenerator,
    IProductVectorStore vectorStore) : IAiSearchIndexingService
{
    public async Task<IndexProductsResult> ReindexProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productIndexSource.GetActiveProductsForIndexAsync(cancellationToken);

        var documents = products
            .Select(product => new ProductSearchDocument(
                product.ProductId,
                product.Name,
                product.Description,
                product.Category,
                product.Price))
            .ToList();

        var embeddings = new List<float[]>(documents.Count);

        foreach (var document in documents)
        {
            var embedding = await embeddingGenerator.GenerateEmbeddingAsync(
                document.TextForEmbedding,
                cancellationToken);

            embeddings.Add(embedding);
        }

        await vectorStore.UpsertProductsAsync(documents, embeddings, cancellationToken);

        return new IndexProductsResult(documents.Count);
    }
}
