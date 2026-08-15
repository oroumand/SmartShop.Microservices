using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartShop.Catalog.Core.Application.Products;

namespace SmartShop.Catalog.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var products = endpoints.MapGroup("/api/catalog/products")
            .WithTags("Catalog");

        products.MapGet("", async (
            IProductQueryService queryService,
            CancellationToken cancellationToken) =>
            Results.Ok(await queryService.GetProductsAsync(cancellationToken)))
            .WithName("GetCatalogProducts")
            .WithSummary("Get active catalog products.");

        products.MapGet("/search", async (
            string? query,
            IProductQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Results.BadRequest("A search query is required.");
            }

            var result = await queryService.SearchProductsAsync(query, cancellationToken);

            return Results.Ok(result);
        })
            .WithName("SearchCatalogProducts")
            .WithSummary("Search active catalog products.");

        products.MapGet("/{id:guid}", async (
            Guid id,
            IProductQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            var product = await queryService.GetProductByIdAsync(id, cancellationToken);

            return product is null
                ? Results.NotFound()
                : Results.Ok(product);
        })
            .WithName("GetCatalogProductById")
            .WithSummary("Get an active catalog product by id.");

        return endpoints;
    }
}
