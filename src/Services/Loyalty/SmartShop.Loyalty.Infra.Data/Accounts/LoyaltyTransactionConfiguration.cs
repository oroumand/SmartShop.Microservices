using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartShop.Loyalty.Core.Domain.Accounts;

namespace SmartShop.Loyalty.Infra.Data.Accounts;

public sealed class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.SourcePaymentId)
            .IsRequired();

        builder.HasIndex(transaction => transaction.SourcePaymentId)
            .IsUnique();

        builder.Property(transaction => transaction.Points)
            .IsRequired();

        builder.Property(transaction => transaction.OccurredAtUtc)
            .IsRequired();

        builder.Property(transaction => transaction.Description)
            .HasMaxLength(250)
            .IsRequired();
    }
}
