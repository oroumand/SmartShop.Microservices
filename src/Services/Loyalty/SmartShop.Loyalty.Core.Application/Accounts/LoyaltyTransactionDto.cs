namespace SmartShop.Loyalty.Core.Application.Accounts;

public sealed record LoyaltyTransactionDto(
    Guid Id,
    Guid SourcePaymentId,
    int Points,
    DateTime OccurredAtUtc,
    string Description);
