namespace SmartShop.ModuleContracts.Ordering;

public interface IOrderingPaymentContract
{
    Task<OrderPaymentInfo?> GetOrderForPaymentAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task MarkOrderAsPaidAsync(
        Guid orderId,
        Guid paymentId,
        CancellationToken cancellationToken = default);
}
