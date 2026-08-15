namespace SmartShop.Ordering.Core.Application.Orders;

public interface IOrderQueryService
{
    Task<OrderDto?> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        CancellationToken cancellationToken = default);
}
