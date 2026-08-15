namespace SmartShop.Payments.Core.Application.Payments;

public interface IPaymentQueryService
{
    Task<PaymentDto> GetPaymentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(
        CancellationToken cancellationToken = default);
}
