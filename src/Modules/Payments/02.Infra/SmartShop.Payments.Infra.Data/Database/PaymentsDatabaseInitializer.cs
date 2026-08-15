using Microsoft.EntityFrameworkCore;

namespace SmartShop.Payments.Infra.Data.Database;

public sealed class PaymentsDatabaseInitializer(PaymentsDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
