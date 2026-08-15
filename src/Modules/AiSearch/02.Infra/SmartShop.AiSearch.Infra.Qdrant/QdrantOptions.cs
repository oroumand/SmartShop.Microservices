namespace SmartShop.AiSearch.Infra.Qdrant;

public sealed class QdrantOptions
{
    public const string SectionName = "AiSearch:Qdrant";

    public string BaseUrl { get; set; } = "http://localhost:6333";

    public string CollectionName { get; set; } = "smartshop-products";

    public int VectorSize { get; set; } = 1536;

    public string Distance { get; set; } = "Cosine";

    public string? ApiKey { get; set; }
}
