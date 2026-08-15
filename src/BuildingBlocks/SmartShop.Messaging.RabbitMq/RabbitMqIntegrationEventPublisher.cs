using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SmartShop.IntegrationEvents;

namespace SmartShop.Messaging.RabbitMq;

public sealed class RabbitMqIntegrationEventPublisher(
    RabbitMqConnection connection,
    IOptions<RabbitMqOptions> options) : IIntegrationEventPublisher, IAsyncDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim _publishGate = new(1, 1);
    private IChannel? _channel;

    public async Task PublishAsync<TEvent>(
        string routingKey,
        TEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);
        ArgumentNullException.ThrowIfNull(integrationEvent);

        await _publishGate.WaitAsync(cancellationToken);

        try
        {
            var channel = await GetChannelAsync();
            var body = JsonSerializer.SerializeToUtf8Bytes(
                integrationEvent,
                SerializerOptions);
            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = integrationEvent.EventId.ToString(),
                Type = typeof(TEvent).Name
            };

            await channel.BasicPublishAsync(
                options.Value.ExchangeName,
                routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }
        finally
        {
            _publishGate.Release();
        }
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        var rabbitConnection = await connection.GetConnectionAsync();
        _channel = await rabbitConnection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(
            options.Value.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        _publishGate.Dispose();
    }
}
