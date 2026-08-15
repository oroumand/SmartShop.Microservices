namespace SmartShop.Payments.Core.Application.Payments;

public interface IPaymentCommandService
{
    Task<PaymentDto> PayOrderAsync(
        PayOrderRequest request,
        CancellationToken cancellationToken = default);
}
