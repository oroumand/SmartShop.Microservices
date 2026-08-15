using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartShop.AiSearch.Core.Application.Search;

namespace SmartShop.AiSearch.Endpoints;

public static class AiSearchEndpoints
{
    public static IEndpointRouteBuilder MapAiSearchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var aiSearch = endpoints.MapGroup("/api/ai-search")
            .WithTags("AiSearch");

        aiSearch.MapPost("/reindex", async (
            IAiSearchIndexingService indexingService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await indexingService.ReindexProductsAsync(cancellationToken);

                return Results.Ok(result);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(exception.Message);
            }
        })
            .WithName("ReindexAiSearchProducts")
            .WithSummary("Reindex products for AI search.")
            .WithDescription("Reads active products through ModuleContracts and stores product vectors in Qdrant.");

        aiSearch.MapGet("/products", async (
            string? query,
            int? limit,
            IAiSearchQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest("A search query is required.");
            }

            try
            {
                var request = new SearchProductsRequest(query, limit.GetValueOrDefault(5));
                var results = await queryService.SearchAsync(request, cancellationToken);

                return Results.Ok(results);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(exception.Message);
            }
        })
            .WithName("SearchAiProducts")
            .WithSummary("Search products with AI search.")
            .WithDescription("Generates an embedding for the query and searches indexed product vectors.");

        return endpoints;
    }
}
