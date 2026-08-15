namespace SmartShop.Loyalty.Core.Application.Accounts;

public interface IEarnLoyaltyPointsService
{
    Task EarnForPaymentAsync(
        EarnPointsForPayment request,
        CancellationToken cancellationToken = default);
}

public sealed record EarnPointsForPayment(
    Guid MessageId,
    Guid CustomerId,
    Guid PaymentId,
    decimal PaidAmount,
    DateTime OccurredAtUtc);
