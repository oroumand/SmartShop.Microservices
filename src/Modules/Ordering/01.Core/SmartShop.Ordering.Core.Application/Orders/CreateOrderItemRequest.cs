namespace SmartShop.Ordering.Core.Application.Orders;

public sealed record CreateOrderItemRequest(
    Guid ProductId,
    int Quantity);
