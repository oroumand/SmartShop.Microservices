using SmartShop.Payments.Core.Application.Payments;
using SmartShop.Payments.Core.Domain.Payments;

namespace SmartShop.Payments.Infra.Data.Payments;

public sealed class EfPaymentRepository(PaymentsDbContext dbContext) : IPaymentRepository
{
    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
