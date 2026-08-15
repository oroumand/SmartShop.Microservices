namespace SmartShop.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(
        string routingKey,
        TEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
