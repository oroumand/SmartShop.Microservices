namespace SmartShop.IntegrationEvents.Payments;

public sealed record PaymentSucceededV1(
    Guid EventId,
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    DateTime OccurredAtUtc) : IIntegrationEvent;
