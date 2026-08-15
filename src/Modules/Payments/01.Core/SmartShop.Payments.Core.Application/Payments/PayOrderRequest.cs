namespace SmartShop.Payments.Core.Application.Payments;

public sealed record PayOrderRequest(
    Guid OrderId,
    string Method);
