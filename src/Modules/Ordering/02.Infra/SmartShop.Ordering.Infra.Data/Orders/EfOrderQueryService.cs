using Microsoft.EntityFrameworkCore;
using SmartShop.Ordering.Core.Application.Orders;
using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Infra.Data.Orders;

public sealed class EfOrderQueryService(OrderingDbContext dbContext) : IOrderQueryService
{
    public async Task<OrderDto?> GetOrderByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

        return order is null ? null : MapToDto(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return orders
            .Select(MapToDto)
            .ToList();
    }

    private static OrderDto MapToDto(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.CustomerName,
            order.CustomerEmail,
            order.Status.ToString(),
            order.CreatedAtUtc,
            order.TotalAmount,
            order.Items
                .Select(item => new OrderItemDto(
                    item.ProductId,
                    item.ProductName,
                    item.UnitPrice,
                    item.Quantity,
                    item.LineTotal))
                .ToList());
}
