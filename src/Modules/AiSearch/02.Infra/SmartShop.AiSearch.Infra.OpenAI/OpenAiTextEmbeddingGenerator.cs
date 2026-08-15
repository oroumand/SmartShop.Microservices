using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SmartShop.AiSearch.Core.Application.Search;

namespace SmartShop.AiSearch.Infra.OpenAI;

public sealed class OpenAiTextEmbeddingGenerator(
    HttpClient httpClient,
    IOptions<OpenAiEmbeddingOptions> options) : ITextEmbeddingGenerator
{
    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var openAiOptions = options.Value;

        if (string.IsNullOrWhiteSpace(openAiOptions.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured. Add AiSearch:OpenAI:ApiKey before using AiSearch endpoints.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/embeddings");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAiOptions.ApiKey);
        request.Content = JsonContent.Create(new EmbeddingRequest(
            openAiOptions.Model,
            text,
            openAiOptions.Dimensions));

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"OpenAI embedding request failed with status code {(int)response.StatusCode}.");
        }

        var embeddingResponse = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(
            cancellationToken);

        var embedding = embeddingResponse?.Data.FirstOrDefault()?.Embedding;

        if (embedding is null || embedding.Length == 0)
        {
            throw new InvalidOperationException(
                "OpenAI returned an invalid embedding response. Expected data[0].embedding to contain numbers.");
        }

        return embedding;
    }

    private sealed record EmbeddingRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("input")] string Input,
        [property: JsonPropertyName("dimensions")] int Dimensions);

    private sealed record EmbeddingResponse(
        [property: JsonPropertyName("data")] IReadOnlyList<EmbeddingData> Data);

    private sealed record EmbeddingData(
        [property: JsonPropertyName("embedding")] float[] Embedding);
}
