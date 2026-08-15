using Microsoft.EntityFrameworkCore;
using SmartShop.Payments.Core.Domain.Payments;
using SmartShop.Payments.Infra.Data.Payments;
using SmartShop.Payments.Infra.Data.Outbox;

namespace SmartShop.Payments.Infra.Data;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
