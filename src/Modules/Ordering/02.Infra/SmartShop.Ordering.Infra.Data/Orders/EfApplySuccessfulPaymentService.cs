using Microsoft.EntityFrameworkCore;
using SmartShop.Ordering.Core.Application.Orders;

namespace SmartShop.Ordering.Infra.Data.Orders;

public sealed class EfApplySuccessfulPaymentService(OrderingDbContext dbContext)
    : IApplySuccessfulPaymentService
{
    public async Task ApplyAsync(
        Guid orderId,
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders.SingleOrDefaultAsync(
            order => order.Id == orderId,
            cancellationToken)
            ?? throw new InvalidOperationException($"Order '{orderId}' was not found.");

        if (order.ApplySuccessfulPayment(paymentId))
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
