using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartShop.IntegrationEvents;
using SmartShop.IntegrationEvents.Payments;

namespace SmartShop.Payments.Infra.Data.Outbox;

public sealed class PaymentOutboxPublisher(
    IServiceScopeFactory scopeFactory,
    ILogger<PaymentOutboxPublisher> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishBatchAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task PublishBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();

        var messages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null)
            .OrderBy(message => message.CreatedAtUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var integrationEvent = JsonSerializer.Deserialize<PaymentSucceededV1>(
                    message.Payload,
                    SerializerOptions)
                    ?? throw new InvalidOperationException("Outbox payload is empty.");

                await publisher.PublishAsync(
                    message.RoutingKey,
                    integrationEvent,
                    cancellationToken);

                message.MarkProcessed();
                logger.LogInformation(
                    "Published outbox message {MessageId} with routing key {RoutingKey}.",
                    message.Id,
                    message.RoutingKey);
            }
            catch (Exception exception)
            {
                message.MarkFailed(exception.Message);
                logger.LogError(
                    exception,
                    "Publishing outbox message {MessageId} failed.",
                    message.Id);
            }
        }

        if (messages.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
