namespace SmartShop.AiSearch.Infra.OpenAI;

public sealed class OpenAiEmbeddingOptions
{
    public const string SectionName = "AiSearch:OpenAI";

    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.openai.com";

    public string Model { get; set; } = "text-embedding-3-small";

    public int Dimensions { get; set; } = 1536;
}
