using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SmartShop.Payments.Core.Application.Payments;

namespace SmartShop.Payments.Endpoints;

public static class PaymentsEndpoints
{
    public static IEndpointRouteBuilder MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
            .WithTags("Payments");

        group.MapGet("", async (
            IPaymentQueryService queryService,
            CancellationToken cancellationToken) =>
            Results.Ok(await queryService.GetPaymentsAsync(cancellationToken)))
            .WithName("GetPayments")
            .WithSummary("Get payments")
            .WithDescription("Returns all payments.");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPaymentQueryService queryService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return Results.Ok(await queryService.GetPaymentByIdAsync(id, cancellationToken));
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        })
            .WithName("GetPaymentById")
            .WithSummary("Get payment by id")
            .WithDescription("Returns a payment by id.");

        group.MapPost("", async (
            PayOrderRequest request,
            IPaymentCommandService commandService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var payment = await commandService.PayOrderAsync(request, cancellationToken);

                return Results.Created($"/api/payments/{payment.Id}", payment);
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
            .WithName("PayOrder")
            .WithSummary("Pay an order")
            .WithDescription("Creates a payment for an existing payable order.");

        return app;
    }
}
