namespace SmartShop.Payments.Core.Domain.Payments;

public sealed class Payment
{
    private Payment()
    {
    }

    private Payment(
        Guid orderId,
        decimal amount,
        PaymentMethod method)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("Order id is required.", nameof(orderId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentMethod Method { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? PaidAtUtc { get; private set; }

    public static Payment CreateRequest(
        Guid orderId,
        decimal amount,
        PaymentMethod method) =>
        new(orderId, amount, method);

    public void MarkAsSucceeded()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as succeeded.");
        }

        Status = PaymentStatus.Succeeded;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending payments can be marked as failed.");
        }

        Status = PaymentStatus.Failed;
    }
}
