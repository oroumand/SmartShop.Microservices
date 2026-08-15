namespace SmartShop.Loyalty.Core.Domain.Accounts;

public sealed class LoyaltyTransaction
{
    private LoyaltyTransaction()
    {
        Description = string.Empty;
    }

    private LoyaltyTransaction(
        Guid loyaltyAccountId,
        Guid sourcePaymentId,
        int points,
        DateTime occurredAtUtc)
    {
        if (loyaltyAccountId == Guid.Empty)
        {
            throw new ArgumentException("Loyalty account id is required.", nameof(loyaltyAccountId));
        }

        if (sourcePaymentId == Guid.Empty)
        {
            throw new ArgumentException("Source payment id is required.", nameof(sourcePaymentId));
        }

        if (points <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(points), "Points must be positive.");
        }

        Id = Guid.NewGuid();
        LoyaltyAccountId = loyaltyAccountId;
        SourcePaymentId = sourcePaymentId;
        Points = points;
        OccurredAtUtc = occurredAtUtc;
        Description = "Points earned from a successful payment.";
    }

    public Guid Id { get; private set; }

    public Guid LoyaltyAccountId { get; private set; }

    public Guid SourcePaymentId { get; private set; }

    public int Points { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    public string Description { get; private set; }

    public static LoyaltyTransaction EarnedFromPayment(
        Guid loyaltyAccountId,
        Guid sourcePaymentId,
        int points,
        DateTime occurredAtUtc) =>
        new(loyaltyAccountId, sourcePaymentId, points, occurredAtUtc);
}
