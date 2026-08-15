namespace SmartShop.Ordering.Core.Application.Orders;

public interface IOrderCommandService
{
    Task<OrderDto> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);
}
