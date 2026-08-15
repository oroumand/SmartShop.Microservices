using Microsoft.EntityFrameworkCore;

namespace SmartShop.Ordering.Infra.Data.Database;

public sealed class OrderingDatabaseInitializer(OrderingDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
