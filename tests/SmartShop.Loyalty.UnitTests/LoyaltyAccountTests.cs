using SmartShop.Loyalty.Core.Domain.Accounts;

namespace SmartShop.Loyalty.UnitTests;

public sealed class LoyaltyAccountTests
{
    [Fact]
    public void Earn_for_payment_adds_one_point_for_each_one_hundred_amount_units()
    {
        var account = new LoyaltyAccount(Guid.NewGuid());

        var transaction = account.EarnForPayment(
            Guid.NewGuid(),
            1_250m,
            DateTime.UtcNow);

        Assert.NotNull(transaction);
        Assert.Equal(12, transaction.Points);
        Assert.Equal(12, account.Balance);
    }

    [Fact]
    public void Earn_for_payment_ignores_amounts_below_one_point_threshold()
    {
        var account = new LoyaltyAccount(Guid.NewGuid());

        var transaction = account.EarnForPayment(
            Guid.NewGuid(),
            99m,
            DateTime.UtcNow);

        Assert.Null(transaction);
        Assert.Equal(0, account.Balance);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void Earn_for_payment_rejects_non_positive_amount()
    {
        var account = new LoyaltyAccount(Guid.NewGuid());

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            account.EarnForPayment(
                Guid.NewGuid(),
                0m,
                DateTime.UtcNow));
    }
}
