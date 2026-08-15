using Microsoft.EntityFrameworkCore;
using SmartShop.Catalog.Infra.Data.Seed;

namespace SmartShop.Catalog.Infra.Data;

public sealed class CatalogDatabaseInitializer(CatalogDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
        await CatalogSeedData.SeedAsync(dbContext, cancellationToken);
    }
}
