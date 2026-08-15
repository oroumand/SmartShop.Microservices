using Microsoft.EntityFrameworkCore;
using SmartShop.Payments.Core.Application.Payments;
using SmartShop.Payments.Core.Domain.Payments;

namespace SmartShop.Payments.Infra.Data.Payments;

public sealed class EfPaymentQueryService(PaymentsDbContext dbContext) : IPaymentQueryService
{
    public async Task<PaymentDto> GetPaymentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .AsNoTracking()
            .SingleOrDefaultAsync(payment => payment.Id == id, cancellationToken);

        return payment is null
            ? throw new InvalidOperationException($"Payment '{id}' was not found.")
            : MapToDto(payment);
    }

    public async Task<IReadOnlyList<PaymentDto>> GetPaymentsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Payments
            .AsNoTracking()
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .Select(payment => MapToDto(payment))
            .ToListAsync(cancellationToken);

    private static PaymentDto MapToDto(Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.CreatedAtUtc,
            payment.PaidAtUtc);
}
