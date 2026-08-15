using SmartShop.Payments.Core.Domain.Payments;

namespace SmartShop.Payments.Core.Application.Payments;

public interface IPaymentRepository
{
    Task<bool> ExistsForOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
