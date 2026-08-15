namespace SmartShop.Ordering.Core.Application.Orders;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    IReadOnlyList<CreateOrderItemRequest> Items);
