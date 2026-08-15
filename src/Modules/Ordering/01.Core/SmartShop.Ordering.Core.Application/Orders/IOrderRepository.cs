using SmartShop.Ordering.Core.Domain.Orders;

namespace SmartShop.Ordering.Core.Application.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
