namespace SmartShop.Payments.Core.Application.Payments;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Method,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);
