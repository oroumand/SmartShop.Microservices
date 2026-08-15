namespace SmartShop.Ordering.Core.Application.Orders;

public interface IApplySuccessfulPaymentService
{
    Task ApplyAsync(
        Guid orderId,
        Guid paymentId,
        CancellationToken cancellationToken = default);
}
