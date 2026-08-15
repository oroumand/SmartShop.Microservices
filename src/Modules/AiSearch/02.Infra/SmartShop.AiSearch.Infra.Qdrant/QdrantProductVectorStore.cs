using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SmartShop.AiSearch.Core.Application.Search;
using SmartShop.AiSearch.Core.Domain.Search;

namespace SmartShop.AiSearch.Infra.Qdrant;

public sealed class QdrantProductVectorStore(
    HttpClient httpClient,
    IOptions<QdrantOptions> options) : IProductVectorStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task UpsertProductsAsync(
        IReadOnlyList<ProductSearchDocument> documents,
        IReadOnlyList<float[]> embeddings,
        CancellationToken cancellationToken = default)
    {
        if (documents.Count != embeddings.Count)
        {
            throw new ArgumentException(
                "The number of product documents must match the number of embeddings.");
        }

        await EnsureCollectionAsync(cancellationToken);

        var points = documents
            .Select((document, index) => new QdrantPoint(
                document.ProductId.ToString("D"),
                embeddings[index],
                new QdrantPayload(
                    document.ProductId.ToString("D"),
                    document.Name,
                    document.Description,
                    document.Category,
                    document.Price)))
            .ToList();

        using var response = await httpClient.PutAsJsonAsync(
            $"/collections/{Uri.EscapeDataString(options.Value.CollectionName)}/points?wait=true",
            new UpsertPointsRequest(points),
            JsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Qdrant upsert failed with status code {(int)response.StatusCode}.");
        }
    }

    public async Task<IReadOnlyList<ProductSearchResult>> SearchProductsAsync(
        float[] queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default)
    {
        await EnsureCollectionAsync(cancellationToken);

        using var response = await httpClient.PostAsJsonAsync(
            $"/collections/{Uri.EscapeDataString(options.Value.CollectionName)}/points/query",
            new QueryPointsRequest(queryEmbedding, limit, true),
            JsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Qdrant search failed with status code {(int)response.StatusCode}.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var points = GetResultPoints(document.RootElement);
        var results = new List<ProductSearchResult>();

        foreach (var point in points)
        {
            results.Add(MapSearchResult(point));
        }

        return results;
    }

    private async Task EnsureCollectionAsync(CancellationToken cancellationToken)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"/collections/{Uri.EscapeDataString(options.Value.CollectionName)}",
            new CreateCollectionRequest(new VectorConfiguration(
                options.Value.VectorSize,
                options.Value.Distance)),
            JsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Qdrant collection setup failed with status code {(int)response.StatusCode}.");
        }
    }

    private static IReadOnlyList<JsonElement> GetResultPoints(JsonElement root)
    {
        if (!root.TryGetProperty("result", out var result))
        {
            throw new InvalidOperationException(
                "Qdrant returned an invalid search response. Expected a result property.");
        }

        if (result.ValueKind == JsonValueKind.Array)
        {
            return result.EnumerateArray().ToList();
        }

        if (result.ValueKind == JsonValueKind.Object &&
            result.TryGetProperty("points", out var points) &&
            points.ValueKind == JsonValueKind.Array)
        {
            return points.EnumerateArray().ToList();
        }

        throw new InvalidOperationException(
            "Qdrant returned an invalid search response. Expected result or result.points to contain search results.");
    }

    private static ProductSearchResult MapSearchResult(JsonElement point)
    {
        if (!point.TryGetProperty("payload", out var payload))
        {
            throw new InvalidOperationException(
                "Qdrant returned a search result without product payload.");
        }

        var productId = ReadRequiredGuid(payload, "productId");
        var name = ReadRequiredString(payload, "name");
        var description = ReadRequiredString(payload, "description");
        var category = ReadRequiredString(payload, "category");
        var price = ReadRequiredDecimal(payload, "price");
        var score = ReadRequiredDouble(point, "score");

        return new ProductSearchResult(
            productId,
            name,
            description,
            category,
            price,
            score);
    }

    private static Guid ReadRequiredGuid(JsonElement element, string propertyName)
    {
        var value = ReadRequiredString(element, propertyName);

        return Guid.TryParse(value, out var guid)
            ? guid
            : throw new InvalidOperationException(
                $"Qdrant payload field '{propertyName}' is not a valid product id.");
    }

    private static string ReadRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) ||
            value.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException(
                $"Qdrant payload field '{propertyName}' is missing or invalid.");
        }

        return value.GetString()!;
    }

    private static decimal ReadRequiredDecimal(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            throw new InvalidOperationException(
                $"Qdrant payload field '{propertyName}' is missing.");
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetDecimal(out var number) => number,
            JsonValueKind.String when decimal.TryParse(
                value.GetString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var textNumber) => textNumber,
            _ => throw new InvalidOperationException(
                $"Qdrant payload field '{propertyName}' is not a valid price.")
        };
    }

    private static double ReadRequiredDouble(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) ||
            value.ValueKind != JsonValueKind.Number ||
            !value.TryGetDouble(out var number))
        {
            throw new InvalidOperationException(
                $"Qdrant result field '{propertyName}' is missing or invalid.");
        }

        return number;
    }

    private sealed record CreateCollectionRequest(
        [property: JsonPropertyName("vectors")] VectorConfiguration Vectors);

    private sealed record VectorConfiguration(
        [property: JsonPropertyName("size")] int Size,
        [property: JsonPropertyName("distance")] string Distance);

    private sealed record UpsertPointsRequest(
        [property: JsonPropertyName("points")] IReadOnlyList<QdrantPoint> Points);

    private sealed record QdrantPoint(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("vector")] float[] Vector,
        [property: JsonPropertyName("payload")] QdrantPayload Payload);

    private sealed record QdrantPayload(
        [property: JsonPropertyName("productId")] string ProductId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("category")] string Category,
        [property: JsonPropertyName("price")] decimal Price);

    private sealed record QueryPointsRequest(
        [property: JsonPropertyName("query")] float[] Query,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("with_payload")] bool WithPayload);
}
