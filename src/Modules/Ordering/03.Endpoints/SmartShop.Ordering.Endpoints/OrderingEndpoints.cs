using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartShop.Ordering.Core.Application.Orders;
using SmartShop.ModuleContracts.Ordering;

namespace SmartShop.Ordering.Endpoints;

public static class OrderingEndpoints
{
    public static IEndpointRouteBuilder MapOrderingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var orders = endpoints.MapGroup("/api/orders")
            .WithTags("Ordering");

        orders.MapGet("", async (
            IOrderQueryService queryService,
            CancellationToken cancellationToken) =>
            Results.Ok(await queryService.GetOrdersAsync(cancellationToken)))
            .WithName("GetOrders")
            .WithSummary("Get orders ordered by newest first.");

        orders.MapGet("/{id:guid}", async (
            Guid id,
            IOrderQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            var order = await queryService.GetOrderByIdAsync(id, cancellationToken);

            return order is null
                ? Results.NotFound()
                : Results.Ok(order);
        })
            .WithName("GetOrderById")
            .WithSummary("Get an order by id.");

        orders.MapPost("", async (
            CreateOrderRequest request,
            IOrderCommandService commandService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                if (request.Items is null || request.Items.Count == 0)
                {
                    return Results.BadRequest("At least one order item is required.");
                }

                var order = await commandService.CreateOrderAsync(request, cancellationToken);

                return Results.Created($"/api/orders/{order.Id}", order);
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(exception.Message);
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(exception.Message);
            }
        })
            .WithName("CreateOrder")
            .WithSummary("Create an order from active catalog products.");

        endpoints.MapGet("/internal/orders/{id:guid}/payment-info", async (
            Guid id,
            IOrderingPaymentContract paymentContract,
            CancellationToken cancellationToken) =>
        {
            var order = await paymentContract.GetOrderForPaymentAsync(
                id,
                cancellationToken);

            return order is null
                ? Results.NotFound()
                : Results.Ok(order);
        })
            .WithName("GetOrderPaymentInfo")
            .WithTags("Ordering/Internal")
            .WithSummary("Internal payment projection for the Payments service.");

        return endpoints;
    }
}
