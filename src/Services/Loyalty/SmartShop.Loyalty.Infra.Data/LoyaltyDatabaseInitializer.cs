using Microsoft.EntityFrameworkCore;

namespace SmartShop.Loyalty.Infra.Data;

public sealed class LoyaltyDatabaseInitializer(LoyaltyDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
