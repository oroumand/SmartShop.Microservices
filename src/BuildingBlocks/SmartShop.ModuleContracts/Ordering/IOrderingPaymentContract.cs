namespace SmartShop.ModuleContracts.Ordering;

public interface IOrderingPaymentContract
{
    Task<OrderPaymentInfo?> GetOrderForPaymentAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}
