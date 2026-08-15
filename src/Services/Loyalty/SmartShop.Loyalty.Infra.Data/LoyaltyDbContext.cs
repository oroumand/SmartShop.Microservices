using Microsoft.EntityFrameworkCore;
using SmartShop.Loyalty.Core.Domain.Accounts;
using SmartShop.Loyalty.Infra.Data.Inbox;

namespace SmartShop.Loyalty.Infra.Data;

public sealed class LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> options)
    : DbContext(options)
{
    public DbSet<LoyaltyAccount> Accounts => Set<LoyaltyAccount>();

    public DbSet<LoyaltyTransaction> Transactions => Set<LoyaltyTransaction>();

    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoyaltyDbContext).Assembly);
    }
}
