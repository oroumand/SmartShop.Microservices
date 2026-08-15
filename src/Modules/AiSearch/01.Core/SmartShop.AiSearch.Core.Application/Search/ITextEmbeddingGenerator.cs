namespace SmartShop.AiSearch.Core.Application.Search;

public interface ITextEmbeddingGenerator
{
    Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}
