using SmartShop.Loyalty.Core.Application.Accounts;

namespace SmartShop.Loyalty.Api;

public static class LoyaltyEndpoints
{
    public static IEndpointRouteBuilder MapLoyaltyEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var loyalty = endpoints.MapGroup("/api/loyalty")
            .WithTags("Loyalty");

        loyalty.MapGet("/customers/{customerId:guid}", async (
            Guid customerId,
            ILoyaltyAccountQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            if (customerId == Guid.Empty)
            {
                return Results.BadRequest("Customer id is required.");
            }

            var account = await queryService.GetAccountAsync(
                customerId,
                cancellationToken);

            return Results.Ok(account);
        })
            .WithName("GetLoyaltyAccount")
            .WithSummary("Get a customer's loyalty balance.");

        loyalty.MapGet("/customers/{customerId:guid}/transactions", async (
            Guid customerId,
            ILoyaltyAccountQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            if (customerId == Guid.Empty)
            {
                return Results.BadRequest("Customer id is required.");
            }

            var transactions = await queryService.GetTransactionsAsync(
                customerId,
                cancellationToken);

            return Results.Ok(transactions);
        })
            .WithName("GetLoyaltyTransactions")
            .WithSummary("Get a customer's loyalty transaction history.");

        return endpoints;
    }
}
