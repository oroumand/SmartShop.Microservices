using System.Text.Json;
using SmartShop.IntegrationEvents;

namespace SmartShop.Payments.Infra.Data.Outbox;

public sealed class EfIntegrationEventOutbox(PaymentsDbContext dbContext)
    : IIntegrationEventOutbox
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public void Enqueue<TEvent>(string routingKey, TEvent integrationEvent)
        where TEvent : IIntegrationEvent
    {
        var message = new OutboxMessage(
            integrationEvent.EventId,
            typeof(TEvent).FullName ?? typeof(TEvent).Name,
            routingKey,
            JsonSerializer.Serialize(integrationEvent, SerializerOptions),
            integrationEvent.OccurredAtUtc);

        dbContext.OutboxMessages.Add(message);
    }
}
