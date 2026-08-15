using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartShop.Loyalty.Core.Domain.Accounts;

namespace SmartShop.Loyalty.Infra.Data.Accounts;

public sealed class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.CustomerId)
            .IsRequired();

        builder.HasIndex(account => account.CustomerId)
            .IsUnique();

        builder.Property(account => account.Balance)
            .IsRequired();

        builder.Property(account => account.CreatedAtUtc)
            .IsRequired();

        builder.HasMany(account => account.Transactions)
            .WithOne()
            .HasForeignKey(transaction => transaction.LoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(account => account.Transactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
