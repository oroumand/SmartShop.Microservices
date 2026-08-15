namespace SmartShop.IntegrationEvents;

public interface IIntegrationEventOutbox
{
    void Enqueue<TEvent>(string routingKey, TEvent integrationEvent)
        where TEvent : IIntegrationEvent;
}
