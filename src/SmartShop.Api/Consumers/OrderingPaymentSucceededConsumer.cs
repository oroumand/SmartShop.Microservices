using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartShop.IntegrationEvents.Payments;
using SmartShop.Messaging.RabbitMq;
using SmartShop.Ordering.Core.Application.Orders;

namespace SmartShop.Api.Consumers;

public sealed class OrderingPaymentSucceededConsumer(
    RabbitMqConnection connection,
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<OrderingPaymentSucceededConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StartConsumerAsync();
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Could not start the Ordering consumer. Retrying.");
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    }

    private async Task StartConsumerAsync()
    {
        var rabbitConnection = await connection.GetConnectionAsync();
        _channel = await rabbitConnection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            options.Value.ExchangeName,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null);
        await _channel.QueueDeclareAsync(
            RabbitMqTopology.OrderingPaymentSucceededQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        await _channel.QueueBindAsync(
            RabbitMqTopology.OrderingPaymentSucceededQueue,
            options.Value.ExchangeName,
            RabbitMqTopology.PaymentSucceededRoutingKey,
            arguments: null);
        await _channel.BasicQosAsync(0, 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleAsync;
        await _channel.BasicConsumeAsync(
            RabbitMqTopology.OrderingPaymentSucceededQueue,
            autoAck: false,
            consumer);
    }

    private async Task HandleAsync(object sender, BasicDeliverEventArgs delivery)
    {
        var channel = _channel
            ?? throw new InvalidOperationException("RabbitMQ channel is not ready.");

        try
        {
            var message = JsonSerializer.Deserialize<PaymentSucceededV1>(
                delivery.Body.Span,
                SerializerOptions)
                ?? throw new InvalidOperationException("Payment event payload is empty.");

            await using var scope = scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider
                .GetRequiredService<IApplySuccessfulPaymentService>();

            await service.ApplyAsync(message.OrderId, message.PaymentId);
            await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false);
            logger.LogInformation(
                "Applied payment event {EventId} for order {OrderId} to Ordering.",
                message.EventId,
                message.OrderId);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ordering failed to process payment event {MessageId}.",
                delivery.BasicProperties.MessageId);
            await channel.BasicNackAsync(
                delivery.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
