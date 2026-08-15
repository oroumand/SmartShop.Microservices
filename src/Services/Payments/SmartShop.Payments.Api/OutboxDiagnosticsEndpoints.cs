using Microsoft.EntityFrameworkCore;
using SmartShop.Payments.Infra.Data;

namespace SmartShop.Payments.Api;

public static class OutboxDiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapOutboxDiagnosticsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/ops/outbox", async (
            PaymentsDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            var pending = dbContext.OutboxMessages
                .Where(message => message.ProcessedAtUtc == null);

            var pendingCount = await pending.CountAsync(cancellationToken);
            var failedCount = await pending.CountAsync(
                message => message.Attempts > 0,
                cancellationToken);
            var oldestCreatedAt = await pending
                .MinAsync(message => (DateTime?)message.CreatedAtUtc, cancellationToken);

            var oldestPendingAgeSeconds = oldestCreatedAt is null
                ? 0
                : Math.Round((DateTime.UtcNow - oldestCreatedAt.Value).TotalSeconds, 1);

            return Results.Ok(new
            {
                pendingCount,
                failedCount,
                oldestPendingAgeSeconds
            });
        })
            .WithName("GetOutboxDiagnostics")
            .WithTags("Operations")
            .WithSummary("Inspect the payment outbox backlog.");

        return endpoints;
    }
}
