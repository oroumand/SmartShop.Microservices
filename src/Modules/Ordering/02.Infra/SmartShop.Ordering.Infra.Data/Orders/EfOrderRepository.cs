using SmartShop.Ordering.Core.Application.Orders;
using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Infra.Data.Orders;

public sealed class EfOrderRepository(OrderingDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
