namespace SmartShop.Ordering.Core.Application.Orders;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);
