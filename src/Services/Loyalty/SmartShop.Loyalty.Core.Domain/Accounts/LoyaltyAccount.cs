namespace SmartShop.Loyalty.Core.Domain.Accounts;

public sealed class LoyaltyAccount
{
    private readonly List<LoyaltyTransaction> _transactions = [];

    private LoyaltyAccount()
    {
    }

    public LoyaltyAccount(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public int Balance { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<LoyaltyTransaction> Transactions => _transactions.AsReadOnly();

    public LoyaltyTransaction? EarnForPayment(
        Guid paymentId,
        decimal paidAmount,
        DateTime occurredAtUtc)
    {
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException("Payment id is required.", nameof(paymentId));
        }

        if (paidAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(paidAmount),
                "Paid amount must be greater than zero.");
        }

        var points = (int)decimal.Floor(paidAmount / 100m);

        if (points == 0)
        {
            return null;
        }

        var transaction = LoyaltyTransaction.EarnedFromPayment(
            Id,
            paymentId,
            points,
            occurredAtUtc);

        Balance += points;
        _transactions.Add(transaction);

        return transaction;
    }
}
