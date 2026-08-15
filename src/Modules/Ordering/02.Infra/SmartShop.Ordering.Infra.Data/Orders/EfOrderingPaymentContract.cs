using Microsoft.EntityFrameworkCore;
using SmartShop.ModuleContracts.Ordering;
using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Infra.Data.Orders;

public sealed class EfOrderingPaymentContract(OrderingDbContext dbContext) : IOrderingPaymentContract
{
    public async Task<OrderPaymentInfo?> GetOrderForPaymentAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == orderId, cancellationToken);

        return order is null
            ? null
            : new OrderPaymentInfo(
                order.Id,
                order.CustomerId,
                order.TotalAmount,
                order.Status.ToString(),
                order.Status == OrderStatus.Pending);
    }
}
